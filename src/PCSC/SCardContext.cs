using System;
using System.Runtime.InteropServices;
using PCSC.Exceptions;
using PCSC.Extensions;
using PCSC.Interop;

namespace PCSC
{
    /// <summary>Manages an application context to the PC/SC Resource Manager.</summary>
    /// <remarks>Each thread of an application shall use its own SCardContext.</remarks>
    public class SCardContext : ISCardContext
    {
        private readonly ISCardApi _api;
        private SCardScope _lastScope;
        private IntPtr _contextPtr;

        /// <inheritdoc />
        public IntPtr Handle => _contextPtr;

        /// <inheritdoc />
        public int MaxAtrSize => MAX_ATR_SIZE;

        /// <inheritdoc />
        public IntPtr Infinite => INFINITE;

        /// <summary>Maximum ATR size.</summary>
        /// <value>
        ///     <list type="table">
        ///         <listheader><term>Platform</term><description>Maximum ATR size</description></listheader>
        ///         <item>
        ///             <term>Windows (Winscard.dll)</term>
        ///             <description>36</description>
        ///         </item>
        ///         <item>
        ///             <term>UNIX/Linux (PC/SClite)</term>
        ///             <description>33</description>
        ///         </item>
        ///     </list>
        /// </value>
        /// <remarks>Attention: Size depends on platform.</remarks>
        // ReSharper disable once InconsistentNaming
        public static int MAX_ATR_SIZE => Platform.Lib.MaxAtrSize;

        /// <summary>Infinite timeout.</summary>
        /// <value>0xFFFFFFFF</value>
        // ReSharper disable once InconsistentNaming
        public static IntPtr INFINITE {
            get {
                // Hack to avoid Overflow exception on Windows 7 32bit
                if (Marshal.SizeOf(typeof(IntPtr)) == 4) {
                    return unchecked((IntPtr) (int) 0xFFFFFFFF);
                }

                return unchecked((IntPtr) 0xFFFFFFFF);
            }
        }

        /// <summary>
        /// Destroys application context to the PC/SC Resource Manager.
        /// </summary>
        ~SCardContext() {
            Dispose(false);
        }

        /// <summary>
        /// Creates a new context instance
        /// </summary>
        public SCardContext() 
        :this (Platform.Lib) {}

        internal SCardContext(ISCardApi api) {
            _api = api;
        }

        /// <inheritdoc />
        public void Establish(SCardScope scope) {
            if (_contextPtr != IntPtr.Zero && IsValid()) {
                Release();
            }

            var rc = _api.EstablishContext(
                scope,
                IntPtr.Zero,
                IntPtr.Zero,
                out var hContext);

            switch (rc) {
                case SCardError.Success:
                    _contextPtr = hContext;
                    _lastScope = scope;
                    break;
                case SCardError.InvalidValue:
                    throw new InvalidScopeTypeException(rc, "Invalid scope type passed");
                default:
                    rc.Throw();
                    break;
            }
        }

        /// <inheritdoc />
        public void Release() {
            if (_contextPtr == IntPtr.Zero) {
                throw new InvalidContextException(SCardError.UnknownError, "Context was not established");
            }

            var rc = _api.ReleaseContext(_contextPtr);

            switch (rc) {
                case SCardError.Success:
                    _contextPtr = IntPtr.Zero;
                    break;
                case SCardError.InvalidHandle:
                    throw new InvalidContextException(rc, "Invalid Context handle");
                case SCardError.InvalidHandleWindows:
                    throw new InvalidContextException(rc, "Invalid Context handle");
                default:
                    rc.Throw();
                    break;
            }
        }

        /// <inheritdoc />
        public ICardHandle Connect(string readerName, SCardShareMode mode, SCardProtocol preferredProtocol) {
            ThrowOnInvalidContext();

            var handle = new CardHandle(_api, this);
            handle.Connect(readerName, mode, preferredProtocol);
            return handle;
        }

        /// <inheritdoc />
        public ICardReader ConnectReader(string readerName, SCardShareMode mode, SCardProtocol preferredProtocol) {
            var handle = Connect(readerName, mode, preferredProtocol);
            return new CardReader(_api, handle, true);
        }

        /// <inheritdoc />
        public SCardError CheckValidity() {
            return _contextPtr == IntPtr.Zero 
                ? SCardError.InvalidHandle 
                : _api.IsValidContext(_contextPtr);
        }

        /// <inheritdoc />
        public bool IsValid() {
            return CheckValidity() == SCardError.Success;
        }

        /// <inheritdoc />
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Disposes a PC/SC application context.</summary>
        /// <param name="disposing">Ignored. If an application context to the PC/SC Resource Manager has been established it will call the <see cref="Release()" /> method.</param>
        protected virtual void Dispose(bool disposing) {
            if (_contextPtr == IntPtr.Zero) return;

            try {
                // free unmanaged resources in order to avoid memleeks.
                Release();
            } catch (InvalidContextException) {
                // RDP connection disconnected?
                // See https://github.com/danm-de/pcsc-sharp/issues/37
            }
        }

        /// <inheritdoc />
        public string[] GetReaders(string[] groups) {
            ThrowOnInvalidContext();

            var rc = _api.ListReaders(
                _contextPtr,
                groups,
                out var readers);

            switch (rc) {
                case SCardError.Success:
                    return readers;
                case SCardError.NoReadersAvailable:
                    return new string[0]; // Service running, no reader connected
                case SCardError.InvalidHandle:
                    throw new InvalidContextException(rc, "Invalid Scope Handle");
                default:
                    rc.Throw();
                    return null;
            }
        }

        /// <inheritdoc />
        public string[] GetReaders() {
            return GetReaders(null);
        }

        /// <inheritdoc />
        public string[] GetReaderGroups() {
            ThrowOnInvalidContext();

            var sc = _api.ListReaderGroups(
                _contextPtr,
                out var groups);

            switch (sc) {
                case SCardError.Success:
                    return groups;
                case SCardError.InvalidHandle:
                    throw new InvalidContextException(sc, "Invalid Scope Handle");
                case SCardError.NoReadersAvailable:
                    return new string[0]; // Service running, no reader connected
                default:
                    sc.Throw();
                    return null;
            }
        }

        /// <inheritdoc />
        public SCardReaderState GetReaderStatus(string readerName) {
            var tmp = (readerName != null)
                ? new[] {readerName}
                : new string[0];

            return GetReaderStatus(tmp)[0];
        }

        /// <inheritdoc />
        public SCardReaderState[] GetReaderStatus(string[] readerNames) {
            if (readerNames == null) {
                throw new ArgumentNullException(nameof(readerNames));
            }

            if (readerNames.Length == 0) {
                throw new ArgumentException("You must specify at least one reader.");
            }

            ThrowOnInvalidContext();

            var states = new SCardReaderState[readerNames.Length];
            for (var i = 0; i < states.Length; i++) {
                states[i] = new SCardReaderState {
                    ReaderName = readerNames[i],
                    CurrentState = SCRState.Unaware
                };
            }

            GetStatusChange(IntPtr.Zero, states)
                .ThrowIfNotSuccess();

            return states;
        }

        /// <inheritdoc />
        public SCardError GetStatusChange(IntPtr timeout, SCardReaderState[] readerStates) {
            ThrowOnInvalidContext();

            return _api.GetStatusChange(_contextPtr, timeout, readerStates);
        }

        /// <inheritdoc />
        public SCardError Cancel() {
            ThrowOnInvalidContext();

            var rc = _api.Cancel(_contextPtr);
            return rc;
        }

        private void ThrowOnInvalidContext() {
            if (_contextPtr.Equals(IntPtr.Zero)) {
                throw new InvalidContextException(SCardError.InvalidHandle);
            }
        }
    }
}
