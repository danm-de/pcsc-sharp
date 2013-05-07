using System;
using PCSC.Interop;

namespace PCSC
{
    public class SCardReader : ISCardReader, IDisposable
    {
        private SCardShareMode _sharemode;
        private SCardContext _context;
        private IntPtr _cardHandle;
        private SCardProtocol _activeprot;
        private string _readername;

        public string ReaderName {
            get { return _readername; }
            protected set { _readername = value; }
        }

        public SCardContext CurrentContext {
            get { return _context; }
            protected set { _context = value; }
        }

        public SCardShareMode CurrentShareMode {
            get { return _sharemode; }
            protected set { _sharemode = value; }
        }

        public SCardProtocol ActiveProtocol {
            get { return _activeprot; }
            protected set { _activeprot = value; }
        }

        public IntPtr CardHandle {
            get { return _cardHandle; }
        }

        public static IntPtr Infinite {
            get { return SCardContext.Infinite; }
        }

        ~SCardReader() {
            Dispose(false);
        }

        public SCardReader(SCardContext context) {
            _context = context;
        }

        public SCardError Connect(string name, SCardShareMode mode, SCardProtocol prefProto) {
            if (name == null) {
                throw new ArgumentNullException("name");
            }

            if (string.IsNullOrWhiteSpace(name)) {
                throw new UnknownReaderException(SCardError.InvalidValue, "Invalid card reader name.");
            }

            if (_context == null || _context._contextPtr.Equals(IntPtr.Zero)) {
                throw new InvalidContextException(SCardError.InvalidHandle, "Invalid connection context.");
            }

            IntPtr hCard;
            SCardProtocol dwActiveProtocol;

            var rc = SCardAPI.Lib.Connect(_context._contextPtr,
                name,
                mode,
                prefProto,
                out hCard,
                out dwActiveProtocol);

            if (rc != SCardError.Success) {
                return rc;
            }

            _cardHandle = hCard;
            _activeprot = dwActiveProtocol;
            _readername = name;
            _sharemode = mode;

            return rc;
        }

        private void ThrowOnInvalidCardHandle() {
            if (_cardHandle.Equals(IntPtr.Zero)) {
                throw new InvalidOperationException(
                    "Reader is currently not connected or no card handle has been returned.");
            }
        }

        public SCardError Disconnect(SCardReaderDisposition discntExec) {
            ThrowOnInvalidCardHandle();

            var rc = SCardAPI.Lib.Disconnect(_cardHandle, discntExec);

            if (rc != SCardError.Success) {
                return rc;
            }

            // reset local variables
            _readername = null;
            _cardHandle = IntPtr.Zero;
            _activeprot = SCardProtocol.Unset;
            _sharemode = SCardShareMode.Shared;

            return rc;
        }

        public SCardError Reconnect(SCardShareMode mode, SCardProtocol prefProto, SCardReaderDisposition initExec) {
            ThrowOnInvalidCardHandle();

            SCardProtocol dwActiveProtocol;
            var rc = SCardAPI.Lib.Reconnect(_cardHandle,
                mode,
                prefProto,
                initExec,
                out dwActiveProtocol);

            if (rc != SCardError.Success) {
                return rc;
            }

            _activeprot = dwActiveProtocol;
            _sharemode = mode;

            return rc;
        }

        public SCardError BeginTransaction() {
            ThrowOnInvalidCardHandle(); 
            
            return SCardAPI.Lib.BeginTransaction(_cardHandle);
        }

        public SCardError EndTransaction(SCardReaderDisposition disposition) {
            ThrowOnInvalidCardHandle();

            return SCardAPI.Lib.EndTransaction(_cardHandle, disposition);
        }

        public SCardError Transmit(SCardPCI ioSendPci, byte[] sendBuffer, SCardPCI ioRecvPci, ref byte[] recvBuffer) {
            if (ioSendPci == null) {
                throw new ArgumentNullException("ioSendPci");
            }
            if (ioSendPci._iomem == IntPtr.Zero) {
                throw new ArgumentException("ioSendPci");
            }

            return Transmit(
                ioSendPci._iomem, 
                sendBuffer, 
                ioRecvPci, 
                ref recvBuffer);
        }


        public SCardError Transmit(IntPtr pioSendPci, byte[] sendBuffer, ref byte[] recvBuffer) {
            return Transmit(
                pioSendPci, 
                sendBuffer, 
                null, 
                ref recvBuffer);
        }

        public SCardError Transmit(IntPtr pioSendPci, byte[] sendBuffer, int sendBufLength, SCardPCI ioRecvPci, byte[] recvBuffer, ref int recvBufLength) {
            ThrowOnInvalidCardHandle();

            var ioRecvPciPtr = IntPtr.Zero;
            if (ioRecvPci != null) {
                ioRecvPciPtr = ioRecvPci._iomem;
            }

            return SCardAPI.Lib.Transmit(
                _cardHandle,
                pioSendPci,
                sendBuffer,
                sendBufLength,
                ioRecvPciPtr,
                recvBuffer,
                ref recvBufLength);
        }

        public SCardError Transmit(IntPtr pioSendPci, byte[] sendBuffer, SCardPCI ioRecvPci, ref byte[] recvBuffer) {
            ThrowOnInvalidCardHandle();

            int pcbRecvLength;
            var ioRecvPciPtr = IntPtr.Zero;
            
            if (ioRecvPci != null) {
                ioRecvPciPtr = ioRecvPci._iomem;
            }

            var rc = SCardAPI.Lib.Transmit(
                _cardHandle,
                pioSendPci,
                sendBuffer,
                ioRecvPciPtr,
                recvBuffer,
                out pcbRecvLength);

            if (rc != SCardError.Success) {
                return rc;
            }

            if (recvBuffer != null && pcbRecvLength < recvBuffer.Length) {
                Array.Resize(ref recvBuffer, pcbRecvLength);
            }

            return rc;
        }

        public SCardError Transmit(byte[] sendBuffer, int sendBufferLength, byte[] recvBuffer, ref int recvBufferLength) {
            SCardPCI iorecvpci = new SCardPCI(); // will be discarded
            return Transmit(
                SCardPCI.GetPci(_activeprot),
                sendBuffer,
                sendBufferLength,
                iorecvpci,
                recvBuffer,
                ref recvBufferLength);
        }

        public SCardError Transmit(byte[] sendBuffer, byte[] recvBuffer, ref int recvBufferLength) {
            var sendbufsize = 0;
            
            if (sendBuffer != null) {
                sendbufsize = sendBuffer.Length;
            }

            return Transmit(
                sendBuffer,
                sendbufsize,
                recvBuffer,
                ref recvBufferLength);
        }

        public SCardError Transmit(byte[] sendBuffer, ref byte[] recvBuffer) {
            var recvbufsize = 0;
            
            if (recvBuffer != null) {
                recvbufsize = recvBuffer.Length;
            }

            var sc = Transmit(
                sendBuffer,
                recvBuffer,
                ref recvbufsize);

            if (sc != SCardError.Success) {
                return sc;
            }
            
            if (recvBuffer != null && (recvbufsize < recvBuffer.Length)) {
                Array.Resize(ref recvBuffer, recvbufsize);
            }

            return sc;
        }

        public SCardError Control(IntPtr controlCode, byte[] sendBuffer, ref byte[] recvBuffer) {
            ThrowOnInvalidCardHandle();

            int lpBytesReturned;

            var rc = SCardAPI.Lib.Control(
                _cardHandle,
                controlCode,
                sendBuffer,
                recvBuffer,
                out lpBytesReturned);

            if (rc != SCardError.Success || recvBuffer == null) {
                return rc;
            }

            if (lpBytesReturned < recvBuffer.Length) {
                Array.Resize(ref recvBuffer, lpBytesReturned);
            }

            return rc;
        }


        public SCardError Status(out string[] readerName, out SCardState state, out SCardProtocol protocol, out byte[] atr) {
            IntPtr dwState;
            IntPtr dwProtocol;

            var rc = SCardAPI.Lib.Status(
                _cardHandle,
                out readerName,
                out dwState,
                out dwProtocol,
                out atr);

            if (rc == SCardError.Success) {
                protocol = SCardHelper.ToProto(dwProtocol);
                state = SCardHelper.ToState(dwState);

                // update local copies 
                _activeprot = protocol;
                if (readerName.Length >= 1) {
                    _readername = readerName[0];
                }
            } else {
                protocol = SCardProtocol.Unset;
                state = SCardState.Unknown;
            }

            return rc;
        }

        public SCardError GetAttrib(SCardAttr attrId, out byte[] pbAttr) {
            return GetAttrib((IntPtr) attrId, out pbAttr);
        }

        public SCardError GetAttrib(IntPtr attrId, out byte[] pbAttr) {
            int attrlen;

            // receive needed size for pbAttr
            var rc = GetAttrib(attrId, null, out attrlen);

            if (rc != SCardError.Success) {
                throw new PCSCException(rc, SCardHelper.StringifyError(rc));
            }

            pbAttr = new byte[attrlen];
            return GetAttrib(attrId, pbAttr, out attrlen);
        }

        public SCardError GetAttrib(SCardAttr attrId, byte[] pbAttr, out int attrLen) {
            return GetAttrib((IntPtr) attrId, pbAttr, out attrLen);
        }

        public SCardError GetAttrib(IntPtr dwAttrId, byte[] pbAttr, out int attrLen) {
            return SCardAPI.Lib.GetAttrib(
                _cardHandle,
                dwAttrId,
                pbAttr,
                out attrLen);
        }

        public SCardError SetAttrib(SCardAttr attr, byte[] pbAttr) {
            return SetAttrib((IntPtr) attr, pbAttr, pbAttr.Length);
        }

        public SCardError SetAttrib(IntPtr attr, byte[] pbAttr) {
            return SetAttrib(attr, pbAttr, pbAttr.Length);
        }

        public SCardError SetAttrib(SCardAttr attr, byte[] pbAttr, int attrBufSize) {
            return SetAttrib((IntPtr) attr, pbAttr, attrBufSize);
        }

        public SCardError SetAttrib(IntPtr attr, byte[] pbAttr, int attrBufSize) {
            return SCardAPI.Lib.SetAttrib(
                _cardHandle,
                attr,
                pbAttr,
                attrBufSize);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }

            if (_cardHandle != IntPtr.Zero) {
                Disconnect(SCardReaderDisposition.Leave);
            }
        }
    }
}
