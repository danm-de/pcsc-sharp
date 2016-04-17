using System;
using System.Globalization;
using System.Threading;

namespace PCSC
{
    /// <summary>Monitors a Smart Card reader and triggers events on status changes.</summary>
    /// <remarks>Creates a new thread and calls the <see cref="M:PCSC.SCardContext.GetStatusChange(System.IntPtr,PCSC.SCardReaderState[])" /> of the given <see cref="T:PCSC.ISCardContext" /> object.</remarks>
    public class SCardMonitor : ISCardMonitor
    {
        private class Monitor
        {
            public Thread Thread;
            public ISCardContext Context;
            public string[] ReaderNames;
            public volatile SCRState[] PreviousStates;
            public volatile IntPtr[] PreviousStateValues;
            public volatile bool CancelRequested;
        }

        private readonly object _gate = new object();

        private static int _monitorCount;

        private readonly ISCardContext _context;
        private readonly bool _releaseContextOnDispose;

        private Monitor _monitor;
        private volatile bool _isDisposed;

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
                var currentMonitor = _monitor;
                return currentMonitor?.ReaderNames;
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
        public bool Monitoring => _monitor != null;

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
        [Obsolete("Use SCardMonitor(IContextFactory, SCardScope) instead. If you do not want to implement your own context factory, use ContextFactory.Instance. This constructor will be removed in the next major release.")]
        public SCardMonitor(ISCardContext context, bool releaseContextOnDispose = false) {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
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
        [Obsolete("Use SCardMonitor(IContextFactory, SCardScope) instead. If you do not want to implement your own context factory, use ContextFactory.Instance. This constructor will be removed in the next major release.")]
        public SCardMonitor(ISCardContext context, SCardScope scope, bool releaseContextOnDispose = true) {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }
            _context = context;
            _context.Establish(scope);
            _releaseContextOnDispose = releaseContextOnDispose;
        }

        /// <summary>Creates a new SCardMonitor object that is able to listen for certain smart card / reader changes.</summary>
        /// <param name="contextFactory">A smartcard context factory</param>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        public SCardMonitor(IContextFactory contextFactory, SCardScope scope) {
            if (contextFactory == null) {
                throw new ArgumentNullException(nameof(contextFactory));
            }

            _context = contextFactory.Establish(scope);
            _releaseContextOnDispose = true;
        }

        /// <summary>Returns the current state of a reader that is currently being monitored.</summary>
        /// <param name="index">The number of the desired reader. The index must be between 0 and (<see cref="P:PCSC.SCardMonitor.ReaderCount" /> - 1).</param>
        /// <returns>The current state of reader with index number <paramref name="index" />.</returns>
        /// <remarks>This method will throw an <see cref="T:System.ArgumentOutOfRangeException" /> if the specified <paramref name="index" /> is invalid. You can enumerate all readers currently monitored with the <see cref="P:PCSC.SCardMonitor.ReaderNames" /> property.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">If the specified <paramref name="index" /> is invalid.</exception>
        public IntPtr GetCurrentStateValue(int index) {
            // actually "previousStateValue" contains the last known value.
            var currentStateValues = _monitor?.PreviousStateValues;
            
            if (currentStateValues == null) {
                throw new InvalidOperationException("Monitor object is not initialized.");
            }
            
            if (index < 0 || (index >= currentStateValues.Length)) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return currentStateValues[index];
        }

        /// <summary>Returns the current state of a reader that is currently being monitored.</summary>
        /// <param name="index">The number of the desired reader. The index must be between 0 and (<see cref="P:PCSC.SCardMonitor.ReaderCount" /> - 1).</param>
        /// <returns>The current state of reader with index number <paramref name="index" />.</returns>
        /// <remarks>This method will throw an <see cref="T:System.ArgumentOutOfRangeException" /> if the specified <paramref name="index" /> is invalid. You can enumerate all readers currently monitored with the <see cref="P:PCSC.SCardMonitor.ReaderNames" /> property.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">If the specified <paramref name="index" /> is invalid.</exception>
        public SCRState GetCurrentState(int index) {
            var previousStates = _monitor?.PreviousStates;

            if (previousStates == null) {
                throw new InvalidOperationException("Monitor object is not initialized.");
            }

            // "previousState" contains the last known value.
            if (index < 0 || (index >= previousStates.Length)) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return previousStates[index];
        }

        /// <summary>Returns the reader name of a given <paramref name="index" />.</summary>
        /// <param name="index">The number of the desired reader. The index must be between 0 and (<see cref="P:PCSC.SCardMonitor.ReaderCount" /> - 1).</param>
        /// <returns>A reader name.</returns>
        /// <remarks>This method will throw an <see cref="T:System.ArgumentOutOfRangeException" /> if the specified <paramref name="index" /> is invalid. You can enumerate all readers currently monitored with the <see cref="P:PCSC.SCardMonitor.ReaderNames" /> property.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">If the specified <paramref name="index" /> is invalid.</exception>
        public string GetReaderName(int index) {
            var currentReaderNames = _monitor?.ReaderNames;

            if (currentReaderNames == null) {
                throw new InvalidOperationException("Monitor object is not initialized.");
            }

            if (index < 0 || (index >= currentReaderNames.Length)) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return currentReaderNames[index];
        }

        /// <summary>The number of readers that currently being monitored.</summary>
        /// <value>Return 0 if no reader is being monitored.</value>
        public int ReaderCount => _monitor?
            .ReaderNames?
            .Length ?? 0;

        /// <summary>Disposes the object.</summary>
        /// <remarks>Dispose will call <see cref="Cancel()" /> in order to stop the background thread. The application context will be disposed if you configured the monitor to do so at construction time.</remarks>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Disposes the object.</summary>
        /// <param name="disposing">Ignored. It will call <see cref="Cancel()" /> in order to stop the background thread. The application context will be disposed if the user configured the monitor to do so at construction time.</param>
        protected virtual void Dispose(bool disposing) {
            if (_isDisposed) {
                return;
            }

            if (disposing) {
                Cancel();
            }

            if (disposing && _releaseContextOnDispose) {
                _context?.Dispose();
            }

            _isDisposed = true;
        }

        /// <summary>Cancels the monitoring of all readers that are currently being monitored.</summary>
        /// <remarks>This will end the monitoring. The method calls the <see cref="ISCardContext.Cancel()" /> method of its Application Context to the PC/SC Resource Manager.</remarks>
        public void Cancel() {
            lock (_gate) {
                var monitor = Interlocked.Exchange(ref _monitor, null);
                if (monitor != null && monitor.Context.IsValid()) {
                    monitor.CancelRequested = true;
                    monitor.Context.Cancel();
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
            if (string.IsNullOrWhiteSpace(readerName)) {
                throw new ArgumentNullException(nameof(readerName));
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
                throw new ArgumentNullException(nameof(readerNames));
            }

            var numberOfReaders = readerNames.Length;
            if (numberOfReaders == 0) {
                throw new ArgumentException("Empty list of reader names.", nameof(readerNames));
            }

            if (_isDisposed) {
                throw new ObjectDisposedException(GetType().FullName);
            }

            lock (_gate) {
                Cancel();

                var context = _context;
                if (!context.IsValid()) {
                    throw new InvalidContextException(SCardError.InvalidHandle,
                        "Connection context object is invalid.");
                }

                var monitorNumber = Interlocked
                    .Increment(ref _monitorCount)
                    .ToString(CultureInfo.InvariantCulture);

                var threadName = string.Concat(GetType().FullName, " #", 
                    monitorNumber);

                var monitor = new Monitor {
                    ReaderNames = readerNames,
                    PreviousStates = new SCRState[numberOfReaders],
                    PreviousStateValues = new IntPtr[numberOfReaders],
                    Context = context,
                    Thread = new Thread(arg => StartMonitor((Monitor)arg)) {
                        IsBackground = true,
                        Name = threadName
                    }
                };

                monitor.Thread.Start(monitor);
                _monitor = monitor;
            }
        }

        private void StartMonitor(Monitor monitor) {

            var readerStates = new SCardReaderState[monitor.ReaderNames.Length];

            for (var i = 0; i < monitor.ReaderNames.Length; i++) {
                readerStates[i] = new SCardReaderState {
                    ReaderName = monitor.ReaderNames[i],
                    CurrentState = SCRState.Unaware
                };
            }

            var rc = monitor.Context.GetStatusChange(IntPtr.Zero, readerStates);

            if (rc == SCardError.Success) {
                // initialize event
                for (var i = 0; i < readerStates.Length; i++) {
                    var initState = readerStates[i].EventState & (~(SCRState.Changed));

                    OnInitialized(readerStates[i].Atr, monitor.ReaderNames[i], initState);

                    monitor.PreviousStates[i] = initState; // remove "Changed"
                    monitor.PreviousStateValues[i] = readerStates[i].EventStateValue;
                }

                while (!_isDisposed && !monitor.CancelRequested) {
                    for (var i = 0; i < readerStates.Length; i++) {
                        readerStates[i].CurrentStateValue = monitor.PreviousStateValues[i];
                    }

                    // block until status change occurs                    
                    rc = monitor.Context.GetStatusChange(monitor.Context.Infinite, readerStates);

                    // Cancel?
                    if (rc != SCardError.Success) {
                        break;
                    }

                    for (var i = 0; i < readerStates.Length; i++) {
                        var newState = readerStates[i].EventState;
                        newState &= (~(SCRState.Changed)); // remove "Changed"

                        var atr = readerStates[i].Atr;
                        var previousState = monitor.PreviousStates[i];
                        var readerName = monitor.ReaderNames[i];

                        // Status change
                        if (previousState != newState) {
                            OnStatusChanged(atr, readerName, previousState, newState);
                        }

                        // Card inserted
                        if (newState.CardIsPresent() && previousState.CardIsAbsent()) {
                            OnCardInserted(atr, readerName, newState);
                        }

                        // Card removed
                        if (newState.CardIsAbsent() && previousState.CardIsPresent()) {
                            OnCardRemoved(atr, readerName, newState);
                        }

                        monitor.PreviousStates[i] = newState;
                        monitor.PreviousStateValues[i] = readerStates[i].EventStateValue;
                    }
                }
            }

            foreach (var state in readerStates) {
                state.Dispose();
            }

            if (_isDisposed || monitor.CancelRequested || rc == SCardError.Cancelled) {
                return;
            }

            OnMonitorException(rc, "An error occured during SCardGetStatusChange(..).");
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

	        try {
		        rc.ThrowIfNotSuccess();
	        } catch (Exception exception) {
		        handler(this, new PCSCException(rc, message, exception));
	        }
        }
    }
}