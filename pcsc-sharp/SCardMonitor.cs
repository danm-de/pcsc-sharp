using System;
using System.Threading;

namespace PCSC
{
    /// <summary>Monitors a Smart Card reader and triggers events on status changes.</summary>
    /// <remarks>Creates a new thread and calls the <see cref="M:PCSC.SCardContext.GetStatusChange(System.IntPtr,PCSC.SCardReaderState[])" /> of the given <see cref="T:PCSC.ISCardContext" /> object.</remarks>
    public class SCardMonitor : ISCardMonitor
    {
        private readonly object _sync = new object();

        private readonly ISCardContext _context;
        private readonly bool _releaseContextOnDispose;
        private SCRState[] _previousState;
        private IntPtr[] _previousStateValue;
        private Thread _monitorthread;
        private string[] _readernames;
        private bool _monitoring;

        /// <summary>A general reader status change.</summary>
        /// <remarks>
        ///     <example>
        ///         <code lang="C#">
        /// // Create a monitor object with its own PC/SC context.
        /// var monitor = new SCardMonitor(
        /// 	new SCardContext(),
        /// 	SCardScope.System,
        ///     true);
        /// 
        /// // Point the callback function(s) to the pre-defined method MyStatusChangedMethod.
        /// monitor.StatusChanged += new StatusChangeEvent(MyStatusChangedMethod);
        /// 
        /// // Start to monitor the reader
        /// monitor.Start("OMNIKEY CardMan 5x21 00 01");
        ///   </code>
        ///     </example>
        /// </remarks>
        public event StatusChangeEvent StatusChanged;

        /// <summary>A new card has been inserted.</summary>
        /// <remarks>
        ///     <example>
        ///         <code lang="C#">
        /// // Create a monitor object with its own PC/SC context.
        /// var monitor = new SCardMonitor(
        /// 	new SCardContext(),
        /// 	SCardScope.System,
        ///     true);
        /// 
        /// monitor.CardInserted += new CardInsertedEvent(MyCardInsertedMethod);
        /// 
        /// // Start to monitor the reader
        /// monitor.Start("OMNIKEY CardMan 5x21 00 01");
        ///   </code>
        ///     </example>
        /// </remarks>
        public event CardInsertedEvent CardInserted;

        /// <summary>A card has been removed.</summary>
        /// <remarks>
        ///     <example>
        ///         <code lang="C#">
        /// // Create a monitor object with its own PC/SC context.
        /// var monitor = new SCardMonitor(
        /// 	new SCardContext(),
        /// 	SCardScope.System,
        ///     true);
        /// 
        /// monitor.CardRemoved += new CardRemovedEvent(MyCardRemovedMethod);
        /// 
        /// // Start to monitor the reader
        /// monitor.Start("OMNIKEY CardMan 5x21 00 01");
        ///   </code>
        ///     </example>
        /// </remarks>
        public event CardRemovedEvent CardRemoved;

        /// <summary>The monitor object has been initialized.</summary>
        /// <remarks>
        ///     <para>This event appears only once for each reader after calling <see cref="SCardMonitor.Start(string)" /> or <see cref="SCardMonitor.Start(string[])" />.</para>
        ///     <example>
        ///         <code lang="C#">
        /// // Create a monitor object with its own PC/SC context.
        /// var monitor = new SCardMonitor(
        /// 	new SCardContext(),
        /// 	SCardScope.System,
        ///     true);
        /// 
        /// monitor.Initialized += new CardInitializedEvent(MyCardInitializedMethod);
        /// 
        /// // Start to monitor the reader
        /// monitor.Start("OMNIKEY CardMan 5x21 00 01");
        ///   </code>
        ///     </example>
        /// </remarks>
        public event CardInitializedEvent Initialized;

        /// <summary>An PC/SC error occurred during monitoring.</summary>
        /// <remarks>
        ///     <example>
        ///         <code lang="C#">
        /// // Create a monitor object with its own PC/SC context.
        /// var monitor = new SCardMonitor(
        /// 	new SCardContext(),
        /// 	SCardScope.System,
        ///     true);
        /// 
        /// monitor.MonitorException += new MonitorExceptionEvent(MyMonitorExceptionMethod);
        /// 
        /// // Start to monitor the reader
        /// monitor.Start("OMNIKEY CardMan 5x21 00 01");
        ///   </code>
        ///     </example>
        /// </remarks>
        public event MonitorExceptionEvent MonitorException;

        /// <summary>All readers that are currently being monitored.</summary>
        /// <value>A <see cref="T:System.String" /> array of reader names. <see langword="null" /> if no readers is being monitored.</value>
        public string[] ReaderNames {
            get {
                if (_readernames == null) {
                    return null;
                }

                var tmp = new string[_readernames.Length];
                Array.Copy(_readernames, tmp, _readernames.Length);

                return tmp;
            }
        }

        /// <summary>Indicates if there are readers currently monitored.</summary>
        /// <value>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Value</term>
        ///             <description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see langword="true" />
        ///             </term>
        ///             <description>Monitoring process ongoing.</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see langword="false" />
        ///             </term>
        ///             <description>No monitoring.</description>
        ///         </item>
        ///     </list>
        /// </value>
        public bool Monitoring {
            get { return _monitoring; }
        }

        /// <summary>
        /// Releases unmanaged resources and stops the background thread (if running).
        /// </summary>
        ~SCardMonitor() {
            Dispose(false);
        }

        /// <summary>Creates a new SCardMonitor object that is able to listen for certain smart card / reader changes.</summary>
        /// <param name="context">A new Application Context to the PC/SC Resource Manager.</param>
        /// <param name="releaseContextOnDispose">If <see langword="true" /> the supplied <paramref name="context" /> will be released (using <see cref="ISCardContext.Release()" />) on <see cref="Dispose()" /></param>
        /// <remarks>The monitor object should use its own application context to the PC/SC Resource Manager. It will create a (new) backgroud thread that will listen for status changes.
        ///     <para>Warning: You MUST dispose the monitor instance otherwise the background thread will run forever!</para>
        /// </remarks>
        public SCardMonitor(ISCardContext context, bool releaseContextOnDispose = false) {
            if (context == null) {
                throw new ArgumentNullException("context");
            }

            _context = context;
            _releaseContextOnDispose = releaseContextOnDispose;
        }

        /// <summary>Creates a new SCardMonitor object that is able to listen for certain smart card / reader changes.</summary>
        /// <param name="context">A new Application Context to the PC/SC Resource Manager.</param>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        /// <param name="releaseContextOnDispose">If <see langword="true" /> the supplied <paramref name="context" /> will be released (using <see cref="ISCardContext.Release()" />) on <see cref="Dispose()" /></param>
        /// <remarks>The monitor object should use its own application context to the PC/SC Resource Manager. It will create a (new) backgroud thread that will listen for status changes.
        ///     <para>Warning: You MUST dispose the monitor instance otherwise the background thread will run forever!</para>
        /// </remarks>
        public SCardMonitor(ISCardContext context, SCardScope scope, bool releaseContextOnDispose = true)
            : this(context, releaseContextOnDispose) {
            _context.Establish(scope);
        }

        /// <summary>Returns the current state of a reader that is currently being monitored.</summary>
        /// <param name="index">The number of the desired reader. The index must be between 0 and (<see cref="P:PCSC.SCardMonitor.ReaderCount" /> - 1).</param>
        /// <returns>The current state of reader with index number <paramref name="index" />.</returns>
        /// <remarks>This method will throw an <see cref="T:System.ArgumentOutOfRangeException" /> if the specified <paramref name="index" /> is invalid. You can enumerate all readers currently monitored with the <see cref="P:PCSC.SCardMonitor.ReaderNames" /> property.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">If the specified <paramref name="index" /> is invalid.</exception>
        public IntPtr GetCurrentStateValue(int index) {
            if (_previousStateValue == null) {
                throw new InvalidOperationException("Monitor object is not initialized.");
            }

            lock (_previousStateValue) {
                // actually "previousStateValue" contains the last known value.
                if (index < 0 || (index > _previousStateValue.Length)) {
                    throw new ArgumentOutOfRangeException("index");
                }
                return _previousStateValue[index];
            }
        }

        /// <summary>Returns the current state of a reader that is currently being monitored.</summary>
        /// <param name="index">The number of the desired reader. The index must be between 0 and (<see cref="P:PCSC.SCardMonitor.ReaderCount" /> - 1).</param>
        /// <returns>The current state of reader with index number <paramref name="index" />.</returns>
        /// <remarks>This method will throw an <see cref="T:System.ArgumentOutOfRangeException" /> if the specified <paramref name="index" /> is invalid. You can enumerate all readers currently monitored with the <see cref="P:PCSC.SCardMonitor.ReaderNames" /> property.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">If the specified <paramref name="index" /> is invalid.</exception>
        public SCRState GetCurrentState(int index) {
            if (_previousState == null) {
                throw new InvalidOperationException("Monitor object is not initialized.");
            }

            lock (_previousState) {
                // "previousState" contains the last known value.
                if (index < 0 || (index > _previousState.Length)) {
                    throw new ArgumentOutOfRangeException("index");
                }
                return _previousState[index];
            }
        }

        /// <summary>Returns the reader name of a given <paramref name="index" />.</summary>
        /// <param name="index">The number of the desired reader. The index must be between 0 and (<see cref="P:PCSC.SCardMonitor.ReaderCount" /> - 1).</param>
        /// <returns>A reader name.</returns>
        /// <remarks>This method will throw an <see cref="T:System.ArgumentOutOfRangeException" /> if the specified <paramref name="index" /> is invalid. You can enumerate all readers currently monitored with the <see cref="P:PCSC.SCardMonitor.ReaderNames" /> property.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">If the specified <paramref name="index" /> is invalid.</exception>
        public string GetReaderName(int index) {
            if (_readernames == null) {
                throw new InvalidOperationException("Monitor object is not initialized.");
            }

            lock (_readernames) {
                if (index < 0 || (index > _readernames.Length)) {
                    throw new ArgumentOutOfRangeException("index");
                }
                return _readernames[index];
            }
        }

        /// <summary>The number of readers that currently being monitored.</summary>
        /// <value>Return 0 if no reader is being monitored.</value>
        public int ReaderCount {
            get {
                lock (_readernames) {
                    return (_readernames == null)
                        ? 0
                        : _readernames.Length;
                }
            }
        }

        /// <summary>Disposes the object.</summary>
        /// <remarks>Dispose will call <see cref="Cancel()" /> in order to stop the background thread. The application context will be disposed if you configured the monitor to do so at construction time.</remarks>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Disposes the object.</summary>
        /// <param name="disposing">Ignored. It will call <see cref="Cancel()" /> in order to stop the background thread. The application context will be disposed if the user configured the monitor to do so at construction time.</param>
        protected virtual void Dispose(bool disposing) {
            Cancel();

            if (disposing && _releaseContextOnDispose && _context != null) {
                _context.Dispose();
            }
        }

        /// <summary>Cancels the monitoring of all readers that are currently being monitored.</summary>
        /// <remarks>This will end the monitoring. The method calls the <see cref="ISCardContext.Cancel()" /> method of its Application Context to the PC/SC Resource Manager.</remarks>
        public void Cancel() {
            lock (_sync) {
                if (!_monitoring) {
                    return;
                }

                _context.Cancel();
                _readernames = null;
                _previousStateValue = null;
                _previousState = null;

                _monitoring = false;
            }
        }

        /// <param name="readerName">The Smart Card reader that shall be monitored.</param>
        /// <summary>Starts to monitor a single Smart Card reader for status changes.</summary>
        /// <remarks>
        ///     <example>
        ///         <code lang="C#">
        /// // Create a new monitor object with its own PC/SC context.
        /// var monitor = new SCardMonitor(
        /// 	new SCardContext(),
        /// 	SCardScope.System,
        ///     true);
        /// 
        /// // Start to monitor a single reader.
        /// monitor.Start("OMNIKEY CardMan 5x21 00 00");
        ///   </code>
        ///     </example>
        ///     <para>Do not forget to register for at least one event:
        ///         <list type="table">
        ///             <listheader><term>Event</term><description>Description</description></listheader>
        ///             <item><term><see cref="E:PCSC.SCardMonitor.CardInserted" /></term><description>A new card has been inserted.</description></item>
        ///             <item><term><see cref="E:PCSC.SCardMonitor.CardRemoved" /></term><description>A card has been removed.</description></item>
        ///             <item><term><see cref="E:PCSC.SCardMonitor.Initialized" /></term><description>Initial status.</description></item>
        ///             <item><term><see cref="E:PCSC.SCardMonitor.StatusChanged" /></term><description>A general status change.</description></item>
        ///             <item><term><see cref="E:PCSC.SCardMonitor.MonitorException" /></term><description>An error occurred.</description></item>
        ///         </list></para>
        /// </remarks>
        public void Start(string readerName) {
            if (readerName.IsNullOrWhiteSpace()) {
                throw new ArgumentNullException("readerName");
            }

            Start(new[] {readerName});
        }

        /// <param name="readerNames">A <see cref="T:System.String" /> array of reader names that shall be monitored.</param>
        /// <summary>Starts to monitor a range Smart Card readers for status changes.</summary>
        /// <remarks>
        ///     <example>
        ///         <code lang="C#">
        /// string [] readerNames;
        /// using (var ctx = new SCardContext()) {
        ///     ctx.Establish(SCardScope.System);
        ///     // Retrieve the names of all installed readers.
        ///     readerNames = ctx.GetReaders();
        ///     ctx.Release();
        /// }
        /// 
        /// // Create a new monitor object with its own PC/SC context.
        /// var monitor = new SCardMonitor(
        /// 	new SCardContext(),
        /// 	SCardScope.System,
        ///     true);
        /// 
        /// foreach (string reader in readerNames) {
        /// 	Console.WriteLine("Start monitoring for reader {0}.", reader);
        /// }
        ///         
        /// // Start monitoring multiple readers.
        /// monitor.Start(readerNames);
        /// </code>
        ///     </example>
        ///     <para>Do not forget to register for at least one event:
        ///         <list type="table">
        ///             <listheader><term>Event</term><description>Description</description></listheader>
        ///             <item><term><see cref="E:PCSC.SCardMonitor.CardInserted" /></term><description>A new card has been inserted.</description></item>
        ///             <item><term><see cref="E:PCSC.SCardMonitor.CardRemoved" /></term><description>A card has been removed.</description></item>
        ///             <item><term><see cref="E:PCSC.SCardMonitor.Initialized" /></term><description>Initial status.</description></item>
        ///             <item><term><see cref="E:PCSC.SCardMonitor.StatusChanged" /></term><description>A general status change.</description></item>
        ///             <item><term><see cref="E:PCSC.SCardMonitor.MonitorException" /></term><description>An error occurred.</description></item>
        ///         </list></para>
        /// </remarks>
        public void Start(string[] readerNames) {
            lock (_sync) {
                if (_monitoring) {
                    Cancel();
                }

                if (readerNames == null) {
                    throw new ArgumentNullException("readerNames");
                }
                if (readerNames.Length == 0) {
                    throw new ArgumentException("Empty list of reader names.", "readerNames");
                }
                if (_context == null || !_context.IsValid()) {
                    throw new InvalidContextException(SCardError.InvalidHandle,
                        "No connection context object specified.");
                }

                _readernames = readerNames;
                _previousState = new SCRState[readerNames.Length];
                _previousStateValue = new IntPtr[readerNames.Length];

                _monitorthread = new Thread(StartMonitor) {
                    IsBackground = true
                };

                _monitorthread.Start();
            }
        }

        private void StartMonitor() {
            _monitoring = true;

            var readerStates = new SCardReaderState[_readernames.Length];

            for (var i = 0; i < _readernames.Length; i++) {
                readerStates[i] = new SCardReaderState {
                    ReaderName = _readernames[i],
                    CurrentState = SCRState.Unaware
                };
            }

            var rc = _context.GetStatusChange(IntPtr.Zero, readerStates);

            if (rc == SCardError.Success) {
                // initialize event
                var onInitializedHandler = Initialized;
                if (onInitializedHandler != null) {
                    for (var i = 0; i < readerStates.Length; i++) {
                        onInitializedHandler(this,
                            new CardStatusEventArgs(
                                _readernames[i],
                                (readerStates[i].EventState & (~(SCRState.Changed))),
                                readerStates[i].Atr));

                        _previousState[i] = readerStates[i].EventState & (~(SCRState.Changed)); // remove "Changed"
                        _previousStateValue[i] = readerStates[i].EventStateValue;
                    }
                }

                while (true) {
                    for (var i = 0; i < readerStates.Length; i++) {
                        readerStates[i].CurrentStateValue = _previousStateValue[i];
                    }

                    // block until status change occurs                    
                    rc = _context.GetStatusChange(_context.Infinite, readerStates);

                    // Cancel?
                    if (rc != SCardError.Success) {
                        break;
                    }

                    for (var i = 0; i < readerStates.Length; i++) {
                        var newState = readerStates[i].EventState;
                        newState &= (~(SCRState.Changed)); // remove "Changed"

                        var atr = readerStates[i].Atr;

                        // Status change
                        var onStatusChangedHandler = StatusChanged;
                        if (onStatusChangedHandler != null && (_previousState[i] != newState)) {
                            onStatusChangedHandler(this,
                                new StatusChangeEventArgs(_readernames[i],
                                    _previousState[i],
                                    newState,
                                    atr));
                        }

                        // Card inserted
                        if (((newState & SCRState.Present) == SCRState.Present) &&
                            ((_previousState[i] & SCRState.Empty) == SCRState.Empty)) {
                            var onCardInsertedHandler = CardInserted;
                            if (onCardInsertedHandler != null) {
                                onCardInsertedHandler(this,
                                    new CardStatusEventArgs(_readernames[i],
                                        newState,
                                        atr));
                            }
                        }

                        // Card removed
                        if (((newState & SCRState.Empty) == SCRState.Empty) &&
                            ((_previousState[i] & SCRState.Present) == SCRState.Present)) {
                            var onCardRemovedHandler = CardRemoved;
                            if (onCardRemovedHandler != null) {
                                onCardRemovedHandler(this,
                                    new CardStatusEventArgs(_readernames[i],
                                        newState,
                                        atr));
                            }
                        }

                        _previousState[i] = newState;
                        _previousStateValue[i] = readerStates[i].EventStateValue;
                    }
                }
            }

            _monitoring = false;

            foreach (var state in readerStates) {
                state.Dispose();
            }

            if (rc == SCardError.Cancelled) {
                return;
            }

            var monitorExceptionHandler = MonitorException;
            if (monitorExceptionHandler != null) {
                monitorExceptionHandler(this, new PCSCException(rc, "An error occured during SCardGetStatusChange(..)."));
            }
        }
    }
}