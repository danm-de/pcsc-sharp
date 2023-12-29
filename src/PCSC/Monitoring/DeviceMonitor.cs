﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PCSC.Exceptions;
using PCSC.Extensions;

namespace PCSC.Monitoring
{
    /// <summary>Monitors for attached and detached smartcard reader devices.</summary>
    public sealed class DeviceMonitor : IDeviceMonitor
    {
        private static int _instanceCounter;

        private readonly IContextFactory _contextFactory;
        private readonly SCardScope _scope;
        private readonly string _instanceName;
        private readonly object _gate = new object();

        private bool _isDisposed;
        private Thread _thread;
        private ISCardContext _ctx;

        /// <summary>
        /// The monitor object has been initialized.
        /// </summary>
        public event DeviceChangeEvent Initialized;

        /// <summary>
        /// New reader(s) have been attached and/or detached.
        /// </summary>
        public event DeviceChangeEvent StatusChanged;

        /// <summary>An PC/SC error occurred during monitoring.</summary>
        public event DeviceMonitorExceptionEvent MonitorException;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="scope"></param>
        public DeviceMonitor(SCardScope scope)
            : this(ContextFactory.Instance, scope) { }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="contextFactory">Context factory used for this monitor</param>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        public DeviceMonitor(IContextFactory contextFactory, SCardScope scope) {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _scope = scope;
            _instanceName = GetInstanceName();
        }

        private string GetInstanceName() {
            var count = Interlocked.Increment(ref _instanceCounter);
            return $"{GetType().Name} #{count}";
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~DeviceMonitor() {
            Dispose(false);
        }

        /// <summary>
        /// Starts monitoring for device status changes
        /// </summary>
        public void Start() {
            if (_isDisposed) {
                throw new ObjectDisposedException(_instanceName);
            }

            lock (_gate) {
                if (_thread != null) return; // already started

                _thread = new Thread(StartMonitor) {
                    Name = _instanceName,
                    IsBackground = true
                };
                _thread.Start();
            }
        }

        private void StartMonitor() {
            try {
                lock (_gate) {
                    _ctx = _contextFactory.Establish(_scope);
                }

                var scannerStates = new[] {
                    new SCardReaderState {
                        ReaderName = "\\\\?PnP?\\Notification",
                        CurrentStateValue = (IntPtr) 0,
                        EventStateValue = (IntPtr) SCRState.Unknown,
                    }
                };

                // get the (current) state known by the smart card resource manager
                var rc = _ctx.GetStatusChange(IntPtr.Zero, scannerStates);
                if (rc != SCardError.Success && rc != SCardError.Timeout) {
                    rc.Throw();
                }

                // get all readers known by the smart card resource manager
                var readers = GetReaders(_ctx);

                OnInitialized(new DeviceChangeEventArgs(
                    readers,
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<string>()));

                while (true) {
                    try {
                        // set (this) application's state to the state known by the smart card resource manager
                        scannerStates[0].CurrentStateValue = scannerStates[0].EventStateValue;
                        scannerStates[0].EventStateValue= (IntPtr) SCRState.Unknown;

                        // wait for state changes reported by SCardGetStatusChange
                        rc = _ctx.GetStatusChange(SCardContext.INFINITE, scannerStates);
                        if (rc == SCardError.Cancelled) {
                            return;
                        }

                        if (rc != SCardError.Success) {
                            rc.Throw();
                        }

                        var previousReaders = readers;

                        /*
                         * The following statement returns a snapshot of the readers currently seen
                         * by the smart card resource manager.
                         *
                         * Some vendors have USB devices that register more than one reader
                         * (e.g. two readers, one for contact and one for contactless cards, as seen
                         * on the OmniKey CardMan 5321).
                         * There is an event for EACH connected/disconnected reader. The EventState
                         * returned by SCardGetStatusChange may already be "deprecated" and
                         * SCardListReaders returns a different reader list.
                         */
                        readers = GetReaders(_ctx);

                        var attached = GetAttachedReaders(previousReaders, readers).ToList();
                        var detached = GetDetachedReaders(previousReaders, readers).ToList();

                        if (attached.Any() || detached.Any()) {
                            OnStatusChanged(new DeviceChangeEventArgs(
                                readers.ToList(),
                                attached,
                                detached));
                        }

                    } catch (NoServiceException) {
                        // Windows 10, service will be restarted or is not available after the last reader has been disconnected
                        Thread.Sleep(1000);
                        
                        lock (_gate) {
                            _ctx?.Dispose();

                            if (_isDisposed || _thread == null) {
                                return;
                            }

                            _ctx = _contextFactory.Establish(_scope);

                            // reset scanner state (assume we do not know the current state)
                            scannerStates[0].CurrentStateValue = (IntPtr)SCRState.Unknown;
                            scannerStates[0].EventStateValue = (IntPtr)SCRState.Unknown;
                        }
                    }
                }
            } catch (Exception exception) {
                OnMonitorException(new DeviceMonitorExceptionEventArgs(exception));
            }
        }

        private static IEnumerable<string> GetAttachedReaders(ICollection<string> old, IEnumerable<string> @new) {
            return @new.Where(reader => !old.Contains(reader));
        }

        private static IEnumerable<string> GetDetachedReaders(IEnumerable<string> old, ICollection<string> @new) {
            return old.Where(reader => !@new.Contains(reader));
        }

        private static HashSet<string> GetReaders(ISCardContext ctx) {
            var readers = ctx.GetReaders() ?? Enumerable.Empty<string>();
            return new HashSet<string>(readers);
        }

        /// <summary>Cancels the monitoring.</summary>
        /// <remarks>This will end the monitoring. The method calls the <see cref="ISCardContext.Cancel()" /> method of its Application Context to the PC/SC Resource Manager.</remarks>
        public void Cancel() {
            if (_isDisposed) return;

            lock (_gate) {
                _ctx?.Cancel();
                _thread = null;
            }
        }

        private void OnMonitorException(DeviceMonitorExceptionEventArgs args) {
            MonitorException?.Invoke(this, args);
        }

        private void OnStatusChanged(DeviceChangeEventArgs e) {
            StatusChanged?.Invoke(this, e);
        }

        private void OnInitialized(DeviceChangeEventArgs e) {
            Initialized?.Invoke(this, e);
        }

        /// <summary>Disposes the object.</summary>
        /// <remarks>Dispose will call <see cref="Cancel()" /> in order to stop the background thread. The application context will be disposed if you configured the monitor to do so at construction time.</remarks>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        private void Dispose(bool disposing) {
            if (_isDisposed) {
                return;
            }

            if (disposing) {
                try {
                    Cancel();
                } catch (InvalidContextException) {
                    // RDP connection disconnected?
                    // See https://github.com/danm-de/pcsc-sharp/issues/37
                }
            }

            _isDisposed = true;
        }
    }
}
