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
        public void Reconnect(SCardShareMode mode, SCardProtocol preferredProtocol, SCardReaderDisposition initialExecution) {
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
        public int Transmit(IntPtr sendPci, byte[] sendBuffer, int sendBufferLength, SCardPCI receivePci, byte[] receiveBuffer,
            int receiveBufferLength) {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public int Control(IntPtr controlCode, byte[] sendBuffer, int sendBufferLength, byte[] receiveBuffer, int receiveBufferSize) {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ReaderStatus GetStatus() {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public int GetAttrib(IntPtr attributeId, byte[] receiveBuffer, int receiveBufferSize) {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void SetAttrib(IntPtr attributeId, byte[] sendBuffer, int sendBufferLength) {
            throw new NotImplementedException();
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
    }
}
