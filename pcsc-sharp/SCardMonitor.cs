using System;
using System.Threading;

namespace PCSC
{
    public delegate void StatusChangeEvent(object sender, StatusChangeEventArgs e);
    public delegate void CardInsertedEvent(object sender, CardStatusEventArgs e);
    public delegate void CardRemovedEvent(object sender, CardStatusEventArgs e);
    public delegate void CardInitializedEvent(object sender, CardStatusEventArgs e);
    public delegate void MonitorExceptionEvent(object sender, PCSCException ex);

    public class SCardMonitor : IDisposable
    {
        public event StatusChangeEvent StatusChanged;
        public event CardInsertedEvent CardInserted;
        public event CardRemovedEvent CardRemoved;
        public event CardInitializedEvent Initialized;
        public event MonitorExceptionEvent MonitorException;

        private readonly object _sync = new object();

        private readonly SCardContext _context;
        private readonly bool _releaseContextOnDispose;
        private SCRState[] _previousState;
        private IntPtr[] _previousStateValue;
        private Thread _monitorthread;
        private string[] _readernames;
        private bool _monitoring;

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

        public bool Monitoring {
            get { return _monitoring; }
        }

        ~SCardMonitor() {
            Dispose(false);
        }

        public SCardMonitor(SCardContext hContext, bool releaseContextOnDispose = false) {
            if (hContext == null) {
                throw new ArgumentNullException("hContext");
            }

            _context = hContext;
            _releaseContextOnDispose = releaseContextOnDispose;
        }

        public SCardMonitor(SCardContext hContext, SCardScope scope, bool releaseContextOnDispose = true)
            : this(hContext, releaseContextOnDispose) {
            
            _context.Establish(scope);
        }

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

        public string GetReaderName(int index) {
            if (_readernames == null)
                throw new InvalidOperationException("Monitor object is not initialized.");

            lock (_readernames) {
                if (index < 0 || (index > _readernames.Length)) {
                    throw new ArgumentOutOfRangeException("index");
                }
                return _readernames[index];
            }
        }

        public int ReaderCount {
            get {
                lock (_readernames) {
                    return (_readernames == null)
                        ? 0
                        : _readernames.Length;
                }
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }

            Cancel();

            if (_releaseContextOnDispose && _context != null) {
                _context.Dispose();
            }
        }

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

        public void Start(string readerName) {
            if (string.IsNullOrWhiteSpace(readerName)) {
                throw new ArgumentNullException("readerName");
            }

            Start(new[] {readerName});
        }

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
                    rc = _context.GetStatusChange(SCardReader.Infinite, readerStates);

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
