using System;
using System.Diagnostics;
using PCSC.Exceptions;
using PCSC.Extensions;
using PCSC.Interop;

namespace PCSC
{
    /// <inheritdoc />
    public sealed class CardHandle : ICardHandle
    {
        private readonly ISCardAPI _api;
        private readonly ISCardContext _context;
        private bool _disposed;

        /// <inheritdoc />
        public string ReaderName { get; private set; }

        /// <inheritdoc />
        public IntPtr Handle { get; private set; }

        /// <inheritdoc />
        public SCardShareMode Mode { get; private set; }

        /// <inheritdoc />
        public SCardProtocol ActiveProtocol { get; private set; }

        /// <inheritdoc />
        public bool IsConnected => Handle != IntPtr.Zero;

        /// <inheritdoc />
        ~CardHandle() {
            Dispose(false);
        }

        /// <summary>
        /// Creates a new <see cref="CardHandle"/> instance
        /// </summary>
        /// <param name="context">The application context to the PC/SC Resource Manager that will be used for <see cref="Connect"/> and <see cref="Disconnect"/></param>
        public CardHandle(ISCardContext context)
            : this(Platform.Lib, context) { }

        internal CardHandle(ISCardAPI api, ISCardContext context) {
            _api = api ?? throw new ArgumentNullException(nameof(api));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc />
        public void Connect(string readerName, SCardShareMode mode, SCardProtocol preferredProtocol) {
            ThrowIfDisposed();
            ThrowIfAlreadyConnected();

            if (readerName == null) {
                throw new ArgumentNullException(nameof(readerName));
            }

            if (string.IsNullOrWhiteSpace(readerName)) {
                throw new UnknownReaderException(SCardError.InvalidValue, "Invalid card reader name.");
            }

            if (_context.Handle.Equals(IntPtr.Zero)) {
                throw new InvalidContextException(SCardError.InvalidHandle, "Invalid connection context.");
            }

            _api.Connect(
                    hContext: _context.Handle,
                    szReader: readerName,
                    dwShareMode: mode,
                    dwPreferredProtocols: preferredProtocol,
                    phCard: out var cardHandle,
                    pdwActiveProtocol: out var activeProtocol)
                .ThrowIfNotSuccess();

            Handle = cardHandle;
            ActiveProtocol = activeProtocol;
            Mode = mode;
            ReaderName = readerName;
        }

        /// <inheritdoc />
        public void Reconnect(SCardShareMode mode, SCardProtocol preferredProtocol,
            SCardReaderDisposition initialExecution) {
            ThrowIfDisposed();
            ThrowOnInvalidCardHandle();

            _api.Reconnect(
                    hCard: Handle,
                    dwShareMode: mode,
                    dwPreferredProtocols: preferredProtocol,
                    dwInitialization: initialExecution,
                    pdwActiveProtocol: out var dwActiveProtocol)
                .ThrowIfNotSuccess();

            ActiveProtocol = dwActiveProtocol;
            Mode = mode;
        }

        /// <inheritdoc />
        public void Disconnect(SCardReaderDisposition disconnectExecution) {
            ThrowIfDisposed();
            ThrowOnInvalidCardHandle();

            _api.Disconnect(
                    hCard: Handle,
                    dwDisposition: disconnectExecution)
                .ThrowIfNotSuccess();

            Handle = IntPtr.Zero;
            ReaderName = null;
            ActiveProtocol = SCardProtocol.Unset;
            Mode = SCardShareMode.Shared;
        }

        /// <inheritdoc />
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void ThrowOnInvalidCardHandle() {
            if (Handle.Equals(IntPtr.Zero)) {
                throw new InvalidOperationException(
                    "Reader is currently not connected or no card handle has been returned.");
            }
        }

        private void ThrowIfAlreadyConnected() {
            if (Handle != IntPtr.Zero) {
                throw new InvalidOperationException(
                    $"Reader is already connected. Use {nameof(Reconnect)}(..) instead.");
            }
        }

        private void ThrowIfDisposed() {
            if (_disposed) throw new ObjectDisposedException(nameof(CardHandle));
        }

        private void Dispose(bool disposing) {
            if (_disposed) return;

            var handle = Handle;

            if (!disposing && handle != IntPtr.Zero) {
                Trace.TraceError("Instance was not disposed. Card handle: {0}", handle);
            }

            if (handle != IntPtr.Zero) {
                SafeDisconnect();
            }

            _disposed = true;
        }

        private void SafeDisconnect() {
            var handle = Handle;
            try {
                Disconnect(SCardReaderDisposition.Leave);
            } catch (Exception exception) {
                Trace.TraceWarning("Could not disconnect card handle {0}: {1} ({2})",
                    handle, exception.Message, exception.GetType());
            }
        }
    }
}
