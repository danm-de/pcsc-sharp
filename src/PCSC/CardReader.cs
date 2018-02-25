using System;
using PCSC.Extensions;
using PCSC.Interop;
using PCSC.Utils;

namespace PCSC
{
    /// <inheritdoc />
    public class CardReader : ICardReader
    {
        private readonly ISCardApi _api;
        private readonly bool _isOwner;
        private bool _disposed;

        /// <inheritdoc />
        public ICardHandle CardHandle { get; }

        /// <inheritdoc />
        public string ReaderName => CardHandle.ReaderName;

        /// <inheritdoc />
        public SCardShareMode Mode => CardHandle.Mode;

        /// <inheritdoc />
        public SCardProtocol Protocol => CardHandle.Protocol;

        /// <inheritdoc />
        public bool IsConnected => CardHandle.IsConnected;

        /// <inheritdoc />
        ~CardReader() {
            Dispose(false);
        }

        /// <summary>
        /// Creates a <see cref="CardReader"/> instance
        /// </summary>
        /// <param name="cardHandle">A connected card/reader handle</param>
        public CardReader(ICardHandle cardHandle)
            : this(cardHandle, true) { }

        /// <summary>
        /// Creates a <see cref="CardReader"/> instance
        /// </summary>
        /// <param name="cardHandle">A connected card/reader handle</param>
        /// <param name="isOwner">If set to <c>true</c>, the reader will destroy the <paramref name="cardHandle"/> on <see cref="Dispose()"/></param>
        public CardReader(ICardHandle cardHandle, bool isOwner)
            : this(Platform.Lib, cardHandle, isOwner) { }

        internal CardReader(ISCardApi api, ICardHandle cardHandle, bool isOwner) {
            _api = api;
            CardHandle = cardHandle ?? throw new ArgumentNullException(nameof(cardHandle));
            _isOwner = isOwner;
        }

        /// <inheritdoc />
        public void Reconnect(SCardShareMode mode, SCardProtocol preferredProtocol,
            SCardReaderDisposition initialExecution) {
            CardHandle.Reconnect(mode, preferredProtocol, initialExecution);
        }

        /// <inheritdoc />
        public IDisposable Transaction(SCardReaderDisposition disposition) {
            var handle = CardHandle.Handle;

            _api.BeginTransaction(handle)
                .ThrowIfNotSuccess();

            return DisposeAction.Create(() => _api
                .EndTransaction(handle, disposition)
                .ThrowIfNotSuccess());
        }

        /// <inheritdoc />
        public int Transmit(byte[] sendBuffer, byte[] receiveBuffer) {
            return Transmit(
                sendPci: SCardPCI.GetPci(Protocol),
                sendBuffer: sendBuffer,
                receiveBuffer: receiveBuffer);
        }

        /// <inheritdoc />
        public int Transmit(SCardPCI sendPci, byte[] sendBuffer, byte[] receiveBuffer) {
            var sendBufferLength = sendBuffer?.Length ?? 0;
            var receiveBufferLength = receiveBuffer?.Length ?? 0;
            return Transmit(
                sendPci: sendPci,
                sendBuffer: sendBuffer,
                sendBufferLength: sendBufferLength,
                receivePci: default(SCardPCI),
                receiveBuffer: receiveBuffer,
                receiveBufferLength: receiveBufferLength);
        }

        /// <inheritdoc />
        public int Transmit(IntPtr sendPci, byte[] sendBuffer, byte[] receiveBuffer) {
            var sendBufferLength = sendBuffer?.Length ?? 0;
            var receiveBufferLength = receiveBuffer?.Length ?? 0;
            return Transmit(
                sendPci: sendPci,
                sendBuffer: sendBuffer,
                sendBufferLength: sendBufferLength,
                receivePci: IntPtr.Zero,
                receiveBuffer: receiveBuffer,
                receiveBufferLength: receiveBufferLength);
        }

        /// <inheritdoc />
        public int Transmit(IntPtr sendPci, byte[] sendBuffer, int sendBufferLength, SCardPCI receivePci,
            byte[] receiveBuffer,
            int receiveBufferLength) {
            var receivePciPointer = receivePci?.MemoryPtr ?? IntPtr.Zero;
            return Transmit(sendPci, sendBuffer, sendBufferLength, receivePciPointer, receiveBuffer,
                receiveBufferLength);
        }

        /// <inheritdoc />
        public int Transmit(SCardPCI sendPci, byte[] sendBuffer, int sendBufferLength, byte[] receiveBuffer,
            int receiveBufferLength) {
            ThrowOnInvalidSendPci(sendPci);

            var sendPciMemoryPtr = sendPci.MemoryPtr;
            return Transmit(sendPciMemoryPtr, sendBuffer, sendBufferLength, IntPtr.Zero, receiveBuffer,
                receiveBufferLength);
        }

        /// <inheritdoc />
        public int Transmit(SCardPCI sendPci, byte[] sendBuffer, int sendBufferLength, SCardPCI receivePci,
            byte[] receiveBuffer,
            int receiveBufferLength) {
            ThrowOnInvalidSendPci(sendPci);

            var sendPciPointer = sendPci.MemoryPtr;
            var receivePciPointer = receivePci?.MemoryPtr ?? IntPtr.Zero;
            return Transmit(sendPciPointer, sendBuffer, sendBufferLength, receivePciPointer, receiveBuffer,
                receiveBufferLength);
        }

        /// <inheritdoc />
        public int Transmit(IntPtr sendPci, byte[] sendBuffer, int sendBufferLength, byte[] receiveBuffer,
            int receiveBufferLength) {
            return Transmit(sendPci, sendBuffer, sendBufferLength, IntPtr.Zero, receiveBuffer, receiveBufferLength);
        }

        /// <inheritdoc />
        public int Transmit(IntPtr sendPci, byte[] sendBuffer, int sendBufferLength, IntPtr receivePci,
            byte[] receiveBuffer,
            int receiveBufferLength) {
            var handle = CardHandle.Handle;
            var bytesReceived = receiveBufferLength;

            _api.Transmit(
                    hCard: handle,
                    pioSendPci: sendPci,
                    pbSendBuffer: sendBuffer,
                    pcbSendLength: sendBufferLength,
                    pioRecvPci: receivePci,
                    pbRecvBuffer: receiveBuffer,
                    pcbRecvLength: ref bytesReceived)
                .ThrowIfNotSuccess();

            return bytesReceived;
        }

        /// <inheritdoc />
        public int Control(IntPtr controlCode, byte[] sendBuffer, byte[] receiveBuffer) {
            var sendBufferLength = sendBuffer?.Length ?? 0;
            var receiveBufferLength = receiveBuffer?.Length ?? 0;
            return Control(controlCode, sendBuffer, sendBufferLength, receiveBuffer, receiveBufferLength);
        }

        /// <inheritdoc />
        public int Control(IntPtr controlCode, byte[] sendBuffer, int sendBufferLength, byte[] receiveBuffer,
            int receiveBufferLength) {
            var handle = CardHandle.Handle;
            _api.Control(handle, controlCode, sendBuffer, sendBufferLength, receiveBuffer, receiveBufferLength,
                    out var bytesReceived)
                .ThrowIfNotSuccess();

            return bytesReceived;
        }

        /// <inheritdoc />
        public ReaderStatus GetStatus() {
            var handle = CardHandle.Handle;
            _api.Status(
                    hCard: handle,
                    szReaderName: out var readerNames,
                    pdwState: out var dwState,
                    pdwProtocol: out var dwProtocol,
                    pbAtr: out var atr)
                .ThrowIfNotSuccess();

            return new ReaderStatus(
                readerNames: readerNames,
                state: SCardHelper.ToState(dwState),
                protocol: SCardHelper.ToProto(dwProtocol),
                atr: atr);
        }

        /// <inheritdoc />
        public int GetAttrib(IntPtr attributeId, byte[] receiveBuffer) {
            var receiveBufferSize = receiveBuffer?.Length ?? 0;
            return GetAttrib(attributeId, receiveBuffer, receiveBufferSize);
        }

        /// <inheritdoc />
        public int GetAttrib(SCardAttribute attributeId, byte[] receiveBuffer, int receiveBufferSize) {
            return GetAttrib((IntPtr) attributeId, receiveBuffer, receiveBufferSize);
        }

        /// <inheritdoc />
        public int GetAttrib(SCardAttribute attributeId, byte[] receiveBuffer) {
            return GetAttrib((IntPtr) attributeId, receiveBuffer);
        }

        /// <inheritdoc />
        public int GetAttrib(IntPtr attributeId, byte[] receiveBuffer, int receiveBufferSize) {
            var handle = CardHandle.Handle;
            _api.GetAttrib(handle, attributeId, receiveBuffer, receiveBufferSize, out var attributeLength)
                .ThrowIfNotSuccess();
            return attributeLength;
        }

        /// <inheritdoc />
        public void SetAttrib(IntPtr attributeId, byte[] sendBuffer) {
            var sendBufferLength = sendBuffer?.Length ?? 0;
            SetAttrib(attributeId, sendBuffer, sendBufferLength);
        }

        /// <inheritdoc />
        public void SetAttrib(SCardAttribute attributeId, byte[] sendBuffer, int sendBufferLength) {
            SetAttrib((IntPtr)attributeId, sendBuffer, sendBufferLength);
        }

        /// <inheritdoc />
        public void SetAttrib(SCardAttribute attributeId, byte[] sendBuffer) {
            SetAttrib((IntPtr)attributeId, sendBuffer);
        }

        /// <inheritdoc />
        public void SetAttrib(IntPtr attributeId, byte[] sendBuffer, int sendBufferLength) {
            var handle = CardHandle.Handle;
            _api.SetAttrib(handle, attributeId, sendBuffer, sendBufferLength)
                .ThrowIfNotSuccess();
        }

        /// <inheritdoc />
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose this instance
        /// </summary>
        /// <param name="disposing">If <c>true</c>, all managed resources will be disposed.</param>
        protected virtual void Dispose(bool disposing) {
            if (!disposing || _disposed) return;

            if (_isOwner) {
                CardHandle.Dispose();
            }

            _disposed = true;
        }

        private static void ThrowOnInvalidSendPci(SCardPCI sendPci) {
            if (sendPci == null) {
                throw new ArgumentNullException(nameof(sendPci));
            }

            if (sendPci.MemoryPtr == IntPtr.Zero) {
                throw new ArgumentException("Valid sendPci required", nameof(sendPci));
            }
        }
    }
}
