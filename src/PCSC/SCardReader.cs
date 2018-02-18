using System;
using PCSC.Exceptions;
using PCSC.Extensions;
using PCSC.Interop;
using PCSC.Utils;

namespace PCSC
{
    /// <summary>A reader class that implements the most basic PC/SC functions to operate on smart cards, RFID tags and so on.</summary>
    /// <remarks>It will use the system's native PC/SC API.</remarks>
    public class SCardReader : ISCardReader
    {
        private IntPtr _cardHandle;

        /// <inheritdoc />
        public string ReaderName { get; protected set; }

        /// <inheritdoc />
        public ISCardContext CurrentContext { get; protected set; }

        /// <inheritdoc />
        public SCardShareMode CurrentShareMode { get; protected set; }

        /// <inheritdoc />
        public SCardProtocol ActiveProtocol { get; protected set; }

        /// <inheritdoc />
        public IntPtr CardHandle => _cardHandle;

        /// <inheritdoc />
        public bool IsConnected => _cardHandle != IntPtr.Zero;

        /// <summary>Unmanaged resources (card handle) are released!</summary>
        ~SCardReader() {
            Dispose(false);
        }

        /// <summary>Creates a new smart card reader object.</summary>
        /// <param name="context">Connection context to the PC/SC Resource Manager.</param>
        /// <remarks>
        ///     <example>
        ///         <code lang="C#">
        /// // Create PC/SC context
        /// var ctx = new SCardContext();
        /// ctx.Establish(SCardScope.System);
        /// 
        /// // Create reader object and connect to the Smart Card
        /// var myReader = new SCardReader(ctx);
        /// var rc = myReader.Connect(
        /// 	"OMNIKEY CardMan 5321", 
        /// 	SCardShareMode.Shared, 
        /// 	SCardProtocol.T1);
        ///   </code>
        ///     </example>
        /// </remarks>
        /// <exception cref="ArgumentNullException">If <paramref name="context"/> is <see langword="null" /></exception>
        public SCardReader(ISCardContext context) {
            CurrentContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc />
        public SCardError Connect(string readerName, SCardShareMode mode, SCardProtocol preferredProtocol) {
            if (readerName == null) {
                throw new ArgumentNullException(nameof(readerName));
            }

            if (string.IsNullOrWhiteSpace(readerName)) {
                throw new UnknownReaderException(SCardError.InvalidValue, "Invalid card reader name.");
            }

            if (CurrentContext == null || CurrentContext.Handle.Equals(IntPtr.Zero)) {
                throw new InvalidContextException(SCardError.InvalidHandle, "Invalid connection context.");
            }

            var rc = Platform.Lib.Connect(CurrentContext.Handle,
                readerName,
                mode,
                preferredProtocol,
                out var hCard,
                out var dwActiveProtocol);

            if (rc != SCardError.Success) {
                return rc;
            }

            _cardHandle = hCard;
            ActiveProtocol = dwActiveProtocol;
            ReaderName = readerName;
            CurrentShareMode = mode;

            return rc;
        }

        private void ThrowOnInvalidCardHandle() {
            if (_cardHandle.Equals(IntPtr.Zero)) {
                throw new InvalidOperationException(
                    "Reader is currently not connected or no card handle has been returned.");
            }
        }

        /// <inheritdoc />
        public SCardError Disconnect(SCardReaderDisposition disconnectExecution) {
            ThrowOnInvalidCardHandle();

            var rc = Platform.Lib.Disconnect(_cardHandle, disconnectExecution);

            if (rc != SCardError.Success) {
                return rc;
            }

            // reset local variables
            ReaderName = null;
            _cardHandle = IntPtr.Zero;
            ActiveProtocol = SCardProtocol.Unset;
            CurrentShareMode = SCardShareMode.Shared;

            return rc;
        }

        /// <inheritdoc />
        public SCardError Reconnect(SCardShareMode mode, SCardProtocol preferredProtocol,
            SCardReaderDisposition initialExecution) {
            ThrowOnInvalidCardHandle();

            var rc = Platform.Lib.Reconnect(_cardHandle,
                mode,
                preferredProtocol,
                initialExecution,
                out var dwActiveProtocol);

            if (rc != SCardError.Success) {
                return rc;
            }

            ActiveProtocol = dwActiveProtocol;
            CurrentShareMode = mode;

            return rc;
        }

        /// <inheritdoc />
        public SCardError BeginTransaction() {
            ThrowOnInvalidCardHandle();

            return Platform.Lib.BeginTransaction(_cardHandle);
        }

        /// <inheritdoc />
        public SCardError EndTransaction(SCardReaderDisposition disposition) {
            ThrowOnInvalidCardHandle();

            return Platform.Lib.EndTransaction(_cardHandle, disposition);
        }

        /// <inheritdoc />
        public SCardError Transmit(SCardPCI sendPci, byte[] sendBuffer, SCardPCI receivePci, ref byte[] receiveBuffer) {
            if (sendPci == null) {
                throw new ArgumentNullException(nameof(sendPci));
            }

            if (sendPci.MemoryPtr == IntPtr.Zero) {
                throw new ArgumentException("sendPci");
            }

            return Transmit(
                sendPci.MemoryPtr,
                sendBuffer,
                receivePci,
                ref receiveBuffer);
        }

        /// <inheritdoc />
        public SCardError Transmit(IntPtr sendPci, byte[] sendBuffer, ref byte[] receiveBuffer) {
            return Transmit(
                sendPci,
                sendBuffer,
                null,
                ref receiveBuffer);
        }

        /// <inheritdoc />
        public SCardError Transmit(IntPtr sendPci, byte[] sendBuffer, int sendBufferLength, SCardPCI receivePci,
            byte[] receiveBuffer, ref int receiveBufferLength) {
            ThrowOnInvalidCardHandle();

            var ioRecvPciPtr = IntPtr.Zero;
            if (receivePci != null) {
                ioRecvPciPtr = receivePci.MemoryPtr;
            }

            return Platform.Lib.Transmit(
                _cardHandle,
                sendPci,
                sendBuffer,
                sendBufferLength,
                ioRecvPciPtr,
                receiveBuffer,
                ref receiveBufferLength);
        }

        /// <inheritdoc />
        public SCardError Transmit(IntPtr sendPci, byte[] sendBuffer, SCardPCI receivePci, ref byte[] receiveBuffer) {
            ThrowOnInvalidCardHandle();

            var ioRecvPciPtr = IntPtr.Zero;

            if (receivePci != null) {
                ioRecvPciPtr = receivePci.MemoryPtr;
            }

            var rc = Platform.Lib.Transmit(
                _cardHandle,
                sendPci,
                sendBuffer,
                ioRecvPciPtr,
                receiveBuffer,
                out var pcbRecvLength);

            if (rc != SCardError.Success) {
                return rc;
            }

            if (receiveBuffer != null && pcbRecvLength < receiveBuffer.Length) {
                Array.Resize(ref receiveBuffer, pcbRecvLength);
            }

            return rc;
        }

        /// <inheritdoc />
        public SCardError Transmit(byte[] sendBuffer, int sendBufferLength, byte[] receiveBuffer,
            ref int receiveBufferLength) {
            var iorecvpci = new SCardPCI(); // will be discarded
            return Transmit(
                SCardPCI.GetPci(ActiveProtocol),
                sendBuffer,
                sendBufferLength,
                iorecvpci,
                receiveBuffer,
                ref receiveBufferLength);
        }

        /// <inheritdoc />
        public SCardError Transmit(byte[] sendBuffer, byte[] receiveBuffer, ref int receiveBufferLength) {
            var sendbufsize = 0;

            if (sendBuffer != null) {
                sendbufsize = sendBuffer.Length;
            }

            return Transmit(
                sendBuffer,
                sendbufsize,
                receiveBuffer,
                ref receiveBufferLength);
        }

        /// <inheritdoc />
        public SCardError Transmit(byte[] sendBuffer, ref byte[] receiveBuffer) {
            var recvbufsize = 0;

            if (receiveBuffer != null) {
                recvbufsize = receiveBuffer.Length;
            }

            var sc = Transmit(
                sendBuffer,
                receiveBuffer,
                ref recvbufsize);

            if (sc != SCardError.Success) {
                return sc;
            }

            if (receiveBuffer != null && (recvbufsize < receiveBuffer.Length)) {
                Array.Resize(ref receiveBuffer, recvbufsize);
            }

            return sc;
        }

        /// <inheritdoc />
        public SCardError Control(IntPtr controlCode, byte[] sendBuffer, ref byte[] receiveBuffer) {
            ThrowOnInvalidCardHandle();


            var rc = Platform.Lib.Control(
                _cardHandle,
                controlCode,
                sendBuffer,
                receiveBuffer,
                out var lpBytesReturned);

            if (rc != SCardError.Success || receiveBuffer == null) {
                return rc;
            }

            if (lpBytesReturned < receiveBuffer.Length) {
                Array.Resize(ref receiveBuffer, lpBytesReturned);
            }

            return rc;
        }

        /// <inheritdoc />
        public SCardError Status(out string[] readerName, out SCardState state, out SCardProtocol protocol,
            out byte[] atr) {

            var rc = Platform.Lib.Status(
                _cardHandle,
                out readerName,
                out var dwState,
                out var dwProtocol,
                out atr);

            if (rc == SCardError.Success) {
                protocol = SCardHelper.ToProto(dwProtocol);
                state = SCardHelper.ToState(dwState);

                // update local copies 
                ActiveProtocol = protocol;
                if (readerName.Length >= 1) {
                    ReaderName = readerName[0];
                }
            } else {
                protocol = SCardProtocol.Unset;
                state = SCardState.Unknown;
            }

            return rc;
        }

        /// <inheritdoc />
        public SCardError GetAttrib(SCardAttribute attributeId, out byte[] attribute) {
            return GetAttrib((IntPtr) attributeId, out attribute);
        }

        /// <inheritdoc />
        public SCardError GetAttrib(IntPtr attributeId, out byte[] attribute) {

            // receive needed size for attribute
            GetAttrib(attributeId, null, out var attrlen)
                .ThrowIfNotSuccess();

            attribute = new byte[attrlen];
            return GetAttrib(attributeId, attribute, out attrlen);
        }

        /// <inheritdoc />
        public SCardError GetAttrib(SCardAttribute attributeId, byte[] attribute, out int attributeBufferLength) {
            return GetAttrib((IntPtr) attributeId, attribute, out attributeBufferLength);
        }

        /// <inheritdoc />
        public SCardError GetAttrib(IntPtr attributeId, byte[] attribute, out int attributeBufferLength) {
            return Platform.Lib.GetAttrib(
                _cardHandle,
                attributeId,
                attribute,
                out attributeBufferLength);
        }

        /// <inheritdoc />
        public SCardError SetAttrib(SCardAttribute attributeId, byte[] attribute) {
            return SetAttrib((IntPtr) attributeId, attribute, attribute.Length);
        }

        /// <inheritdoc />
        public SCardError SetAttrib(IntPtr attributeId, byte[] attribute) {
            return SetAttrib(attributeId, attribute, attribute.Length);
        }

        /// <inheritdoc />
        public SCardError SetAttrib(SCardAttribute attributeId, byte[] attribute, int attributeBufferLength) {
            return SetAttrib((IntPtr) attributeId, attribute, attributeBufferLength);
        }

        /// <inheritdoc />
        public SCardError SetAttrib(IntPtr attributeId, byte[] attribute, int attributeBufferLength) {
            return Platform.Lib.SetAttrib(
                _cardHandle,
                attributeId,
                attribute,
                attributeBufferLength);
        }

        /// <inheritdoc />
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>This will disconnect the reader if it is currently connected using <see cref="SCardReaderDisposition.Leave" />.</summary>
        /// <param name="disposing">Ignored. The reader will be disconnected.</param>
        protected virtual void Dispose(bool disposing) {
            if (_cardHandle != IntPtr.Zero) {
                Disconnect(SCardReaderDisposition.Leave);
            }
        }
    }
}
