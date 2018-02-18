using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PCSC.Exceptions;

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
        private ISCardContext _ctx;
        private Thread _thread;

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
        /// <param name="contextFactory">Context factory used for this monitor</param>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        public DeviceMonitor(IContextFactory contextFactory, SCardScope scope) {
            _instanceName = GetInstanceName();
            _contextFactory = contextFactory;
            _scope = scope;
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
                var ctx = _contextFactory.Establish(_scope);
                var oldCtx = Interlocked.Exchange(ref _ctx, ctx);

                if (oldCtx != null) {
                    oldCtx.Cancel();
                    oldCtx.Dispose();
                }

                _thread = new Thread(() => StartMonitor(ctx)) {
                    Name = _instanceName,
                    IsBackground = true
                };
                _thread.Start();
            }
        }

        private void StartMonitor(ISCardContext ctx) {
            try {
                var readers = GetReaders(ctx);
                OnInitialized(new DeviceChangeEventArgs(
                    readers, 
                    Enumerable.Empty<string>(), 
                    Enumerable.Empty<string>()));

                while (true) {
                    var scannerStates = new[] {
                        new SCardReaderState {
                            ReaderName = "\\\\?PnP?\\Notification",
                            CurrentStateValue = (IntPtr) (readers.Count << 16),
                            EventStateValue = (IntPtr) SCRState.Unknown,
                        }
                    };

                    var rc = ctx.GetStatusChange(ctx.Infinite, scannerStates);
                    if (rc == SCardError.Cancelled) {
                        return;
                    }

                    if (rc != SCardError.Success) {
                        throw new PCSCException(rc);
                    }

                    var newReaderList = GetReaders(ctx);
                    var attached = GetAttachedReaders(readers, newReaderList);
                    var detached = GetDetachedReaders(readers, newReaderList);

                    OnStatusChanged(new DeviceChangeEventArgs(
                        newReaderList.ToList(), 
                        attached.ToList(), 
                        detached.ToList()));

                    readers = newReaderList;
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
                var oldCtx = Interlocked.Exchange(ref _ctx, null);
                if (oldCtx == null) {
                    return;
                }

                oldCtx.Cancel();
                oldCtx.Dispose();
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
