using System;
using System.Globalization;
using System.Threading;

namespace PCSC
{
    /// <summary>Monitors a Smart Card reader and triggers events on status changes.</summary>
    /// <remarks>Creates a new thread and calls the <see cref="M:PCSC.SCardContext.GetStatusChange(System.IntPtr,PCSC.SCardReaderState[])" /> of the given <see cref="T:PCSC.ISCardContext" /> object.</remarks>
    public class SCardMonitor : ISCardMonitor
    {
        private static readonly TimeSpan CANCEL_MAX_WAIT_TIME = TimeSpan.FromSeconds(30);
        private readonly object _sync = new object();

        private static int _monitor_count;

        private readonly ISCardContext _context;
        private readonly bool _releaseContextOnDispose;
        private Thread _monitorthread;
        
        private volatile SCRState[] _previousStates;
        private volatile IntPtr[] _previousStateValues;
        private volatile string[] _readernames;
        private volatile bool _monitoring;
        private volatile bool _is_disposed;

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
                var currentReaderNames = _readernames;

                if (currentReaderNames == null) {
                    return null;
                }

                var tmp = new string[currentReaderNames.Length];
                Array.Copy(currentReaderNames, tmp, currentReaderNames.Length);

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
            // actually "previousStateValue" contains the last known value.
            var currentStateValues = _previousStateValues;
            
            if (currentStateValues == null) {
                throw new InvalidOperationException("Monitor object is not initialized.");
            }
            
            if (index < 0 || (index > currentStateValues.Length)) {
                throw new ArgumentOutOfRangeException("index");
            }

            return currentStateValues[index];
        }

        /// <summary>Returns the current state of a reader that is currently being monitored.</summary>
        /// <param name="index">The number of the desired reader. The index must be between 0 and (<see cref="P:PCSC.SCardMonitor.ReaderCount" /> - 1).</param>
        /// <returns>The current state of reader with index number <paramref name="index" />.</returns>
        /// <remarks>This method will throw an <see cref="T:System.ArgumentOutOfRangeException" /> if the specified <paramref name="index" /> is invalid. You can enumerate all readers currently monitored with the <see cref="P:PCSC.SCardMonitor.ReaderNames" /> property.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">If the specified <paramref name="index" /> is invalid.</exception>
        public SCRState GetCurrentState(int index) {
            if (_previousStates == null) {
                throw new InvalidOperationException("Monitor object is not initialized.");
            }

            lock (_previousStates) {
                // "previousState" contains the last known value.
                if (index < 0 || (index > _previousStates.Length)) {
                    throw new ArgumentOutOfRangeException("index");
                }
                return _previousStates[index];
            }
        }

        /// <summary>Returns the reader name of a given <paramref name="index" />.</summary>
        /// <param name="index">The number of the desired reader. The index must be between 0 and (<see cref="P:PCSC.SCardMonitor.ReaderCount" /> - 1).</param>
        /// <returns>A reader name.</returns>
        /// <remarks>This method will throw an <see cref="T:System.ArgumentOutOfRangeException" /> if the specified <paramref name="index" /> is invalid. You can enumerate all readers currently monitored with the <see cref="P:PCSC.SCardMonitor.ReaderNames" /> property.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">If the specified <paramref name="index" /> is invalid.</exception>
        public string GetReaderName(int index) {
            var currentReaderNames = _readernames;

            if (currentReaderNames == null) {
                throw new InvalidOperationException("Monitor object is not initialized.");
            }

            if (index < 0 || (index > currentReaderNames.Length)) {
                throw new ArgumentOutOfRangeException("index");
            }
            return currentReaderNames[index];
        }

        /// <summary>The number of readers that currently being monitored.</summary>
        /// <value>Return 0 if no reader is being monitored.</value>
        public int ReaderCount {
            get {
                var currentReaderNames = _readernames;
                
                return (currentReaderNames == null)
                    ? 0
                    : currentReaderNames.Length;
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
            if (_is_disposed) {
                return;
            }

            if (disposing) {
                Cancel();
            }

            if (disposing && _releaseContextOnDispose && _context != null) {
                _context.Dispose();
            }

            _is_disposed = true;
        }

        /// <summary>Cancels the monitoring of all readers that are currently being monitored.</summary>
        /// <remarks>This will end the monitoring. The method calls the <see cref="ISCardContext.Cancel()" /> method of its Application Context to the PC/SC Resource Manager.</remarks>
        public void Cancel() {
            lock (_sync) {
                if (_monitoring && _context.IsValid()) {
                    _context.Cancel();
                }
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
            if (readerNames == null) {
                throw new ArgumentNullException("readerNames");
            }
            if (readerNames.Length == 0) {
                throw new ArgumentException("Empty list of reader names.", "readerNames");
            }
            if (_is_disposed) {
                throw new ObjectDisposedException(GetType().FullName);
            }

            lock (_sync) {
                if (_monitoring) {
                    StopPreviousMonitoringThread();    
                }
                
                if (!_context.IsValid()) {
                    throw new InvalidContextException(SCardError.InvalidHandle,
                        "Connection context object is invalid.");
                }

                _readernames = readerNames;
                _previousStates = new SCRState[readerNames.Length];
                _previousStateValues = new IntPtr[readerNames.Length];

                var monitorNumber = Interlocked
                    .Increment(ref _monitor_count)
                    .ToString(CultureInfo.InvariantCulture);

                var threadName = string.Concat(GetType().FullName, " #", monitorNumber);

                _monitorthread = new Thread(StartMonitor) {
                    IsBackground = true,
                    Name = threadName
                };

                _monitorthread.Start();
            }
        }

        private void StopPreviousMonitoringThread() {
            var oldThread = _monitorthread;
            
            Cancel();

            if (oldThread == null) {
                return;
            }

            if (oldThread == Thread.CurrentThread) {
                const string MESSAGE = "Cannot (re-)start monitor within its own thread. " 
                                       + "Hint: Check if your code tries to start the monitor inside of StatusChanged, " 
                                       + "CardInserted, CardRemoved, Initialized or MonitorException. This is not allowed!";
                throw new InvalidOperationException(MESSAGE);
            }

            if (oldThread.Join(CANCEL_MAX_WAIT_TIME)) {
                return;
            }

            throw new TimeoutException("Could not stop monitor thread.");
        }

        private void StartMonitor() {
            try {
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
                    for (var i = 0; i < readerStates.Length; i++) {
                        var initState = readerStates[i].EventState & (~(SCRState.Changed));
                        
                        OnInitialized(readerStates[i].Atr, _readernames[i], initState);

                        _previousStates[i] = initState; // remove "Changed"
                        _previousStateValues[i] = readerStates[i].EventStateValue;
                    }

                    while (!_is_disposed) {
                        for (var i = 0; i < readerStates.Length; i++) {
                            readerStates[i].CurrentStateValue = _previousStateValues[i];
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
                            var previousState = _previousStates[i];
                            var readerName = _readernames[i];

                            // Status change
                            if (previousState != newState) {
                               new Thread(() => OnStatusChanged(atr, readerName, previousState, newState)).Start();
                            }

                            // Card inserted
                            if (newState.CardIsPresent() && previousState.CardIsAbsent()) {
                                new Thread(() => OnCardInserted(atr, readerName, newState)).Start();
                            }

                            // Card removed
                            if (newState.CardIsAbsent() && previousState.CardIsPresent()) {
                                new Thread(() => OnCardRemoved(atr, readerName, newState)).Start();
                            }

                            _previousStates[i] = newState;
                            _previousStateValues[i] = readerStates[i].EventStateValue;
                        }
                    }
                }

                foreach (var state in readerStates) {
                    state.Dispose();
                }

                if (!_is_disposed && (rc != SCardError.Cancelled)) {
                    OnMonitorException(rc, "An error occured during SCardGetStatusChange(..).");
                }

            } finally {
                _readernames = null;
                _previousStateValues = null;
                _previousStates = null;

                _monitoring = false;
                _monitorthread = null;
            }
        }

        private void OnInitialized(byte[] atr, string readerName, SCRState initState) {
            var handler = Initialized;

            if (handler == null) {
                return;
            }
            
            var args = new CardStatusEventArgs(
                readerName,
                initState,
                atr);

            handler(this, args);
        }

        private void OnCardRemoved(byte[] atr, string readerName, SCRState newState) {
            var handler = CardRemoved;
            
            if (handler == null) {
                return;
            }

            var args = new CardStatusEventArgs(
                readerName,
                newState,
                atr);

            handler(this, args);
        }

        private void OnCardInserted(byte[] atr, string readerName, SCRState newState) {
            var handler = CardInserted;
            
            if (handler == null) {
                return;
            }

            var args = new CardStatusEventArgs(
                readerName,
                newState,
                atr);

            handler(this, args);
        }

        private void OnStatusChanged(byte[] atr, string readerName, SCRState previousState, SCRState newState) {
            var handler = StatusChanged;
            
            if (handler == null) {
                return;
            }

            var args = new StatusChangeEventArgs(
                readerName,
                previousState,
                newState,
                atr);

            handler(this, args);
        }

        private void OnMonitorException(SCardError rc, string message) {
            var handler = MonitorException;
            
            if (handler == null) {
                return;
            }

            var exception = new PCSCException(rc, message);
            handler(this, exception);
        }
    }
}