using System;
using System.Runtime.InteropServices;

using PCSC.Interop;

namespace PCSC
{
    public class SCardContext : IDisposable
    {
        private bool _hasContext;
        private SCardScope _lastScope;
        private IntPtr _contextPtr;

        ~SCardContext() {
            Dispose(false);
        }

        public void Establish(SCardScope scope) {
            if (_hasContext && IsValid()) {
                Release();
            }

            IntPtr hContext;

            var rc = Platform.Lib.EstablishContext(
                scope,
                IntPtr.Zero,
                IntPtr.Zero,
                out hContext);

            switch (rc) {
                case SCardError.Success:
                    _contextPtr = hContext;
                    _lastScope = scope;
                    _hasContext = true;
                    break;
                case SCardError.InvalidValue:
                    throw new InvalidScopeTypeException(rc, "Invalid scope type passed");
                default:
                    throw new PCSCException(rc, SCardHelper.StringifyError(rc));
            }
        }

        public void Release() {
            if (!_hasContext) {
                throw new InvalidContextException(SCardError.UnknownError, "Context was not established");
            }

            var rc = Platform.Lib.ReleaseContext(_contextPtr);

            switch (rc) {
                case SCardError.Success:
                    _contextPtr = IntPtr.Zero;
                    _hasContext = false;
                    break;
                case SCardError.InvalidHandle:
                    throw new InvalidContextException(rc, "Invalid Context handle");
                default:
                    throw new PCSCException(rc, SCardHelper.StringifyError(rc));
            }
        }

        public SCardError CheckValidity() {
            return Platform.Lib.IsValidContext(_contextPtr);
        }

        public bool IsValid() {
            return CheckValidity() == SCardError.Success;
        }

        public void ReEstablish() {
            Establish(_lastScope);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }

            if (_hasContext) {
                Release();
            }
        }

        public string[] GetReaders(string[] groups) {
            if (_contextPtr.Equals(IntPtr.Zero)) {
                throw new InvalidContextException(SCardError.InvalidHandle);
            }

            string[] readers;
            var rc = Platform.Lib.ListReaders(
                _contextPtr,
                groups,
                out readers);

            switch (rc) {
                case SCardError.Success:
                    return readers;
                case SCardError.InvalidHandle:
                    throw new InvalidContextException(rc, "Invalid Scope Handle");
                default:
                    throw new PCSCException(rc, SCardHelper.StringifyError(rc));
            }
        }

        public string[] GetReaders() {
            return GetReaders(null);
        }

        public string[] GetReaderGroups() {
            if (_contextPtr.Equals(IntPtr.Zero)) {
                throw new InvalidContextException(SCardError.InvalidHandle);
            }

            string[] groups;

            var sc = Platform.Lib.ListReaderGroups(
                _contextPtr,
                out groups);

            switch (sc) {
                case SCardError.Success:
                    return groups;
                case SCardError.InvalidHandle:
                    throw new InvalidContextException(sc, "Invalid Scope Handle");
                default:
                    throw new PCSCException(sc, SCardHelper.StringifyError(sc));
            }
        }

        public SCardReaderState GetReaderStatus(string readerName) {
            var tmp = (readerName != null)
                ? new[] {readerName}
                : new string[0];

            return GetReaderStatus(tmp)[0];
        }

        public SCardReaderState[] GetReaderStatus(string[] readerNames) {
            if (readerNames == null) {
                throw new ArgumentNullException("readerNames");
            }
            if (readerNames.Length == 0) {
                throw new ArgumentException("You must specify at least one reader.");
            }

            var states = new SCardReaderState[readerNames.Length];
            for (var i = 0; i < states.Length; i++) {
                states[i] = new SCardReaderState {
                    ReaderName = readerNames[i], 
                    CurrentState = SCRState.Unaware
                };
            }

            var rc = GetStatusChange(IntPtr.Zero, states);
            
            if (rc != SCardError.Success) {
                throw new PCSCException(rc, SCardHelper.StringifyError(rc));
            }

            return states;
        }

        public SCardError GetStatusChange(IntPtr timeout, SCardReaderState[] readerStates) {
            if (_contextPtr.Equals(IntPtr.Zero)) {
                throw new InvalidContextException(SCardError.InvalidHandle);
            }

            return Platform.Lib.GetStatusChange(_contextPtr, timeout, readerStates);
        }

        public SCardError Cancel() {
            if (_contextPtr.Equals(IntPtr.Zero)) {
                throw new InvalidContextException(SCardError.UnknownError, "Invalid connection context.");
            }

            var rc = Platform.Lib.Cancel(_contextPtr);
            return rc;
        }

        public static int MaxAtrSize {
            get { return Platform.Lib.MaxAtrSize; }
        }

        public IntPtr Handle {
            get { return _contextPtr; }
        }

        public static IntPtr Infinite {
            get {
                // Hack to avoid Overflow exception on Windows 7 32bit
                if (Marshal.SizeOf(typeof(IntPtr)) == 4) {
                    return unchecked((IntPtr) (Int32) 0xFFFFFFFF);
                }
                return unchecked((IntPtr) 0xFFFFFFFF);
            }
        }
    }
}
