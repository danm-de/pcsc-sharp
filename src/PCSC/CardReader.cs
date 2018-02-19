using System;
using PCSC.Interop;

namespace PCSC
{
    /// <summary>A reader class that implements the most basic PC/SC functions to operate on smart cards, RFID tags and so on.</summary>
    public class CardReader : IDisposable
    {
        private readonly ISCardAPI _api;
        private readonly ICardHandle _handle;
        private readonly bool _isOwner;
        private bool _disposed;

        /// <summary>
        /// Creates a <see cref="CardReader"/> instance
        /// </summary>
        /// <param name="connection">A connected card/reader handle</param>
        public CardReader(ICardHandle connection)
            : this(connection, true) { }

        /// <summary>
        /// Creates a <see cref="CardReader"/> instance
        /// </summary>
        /// <param name="connection">A connected card/reader handle</param>
        /// <param name="isOwner">If set to <c>true</c>, the reader will destroy the <paramref name="connection"/> on <see cref="Dispose()"/></param>
        public CardReader(ICardHandle connection, bool isOwner)
            : this(Platform.Lib, connection, isOwner) { }

        internal CardReader(ISCardAPI api, ICardHandle connection, bool isOwner) {
            _api = api;
            _handle = connection ?? throw new ArgumentNullException(nameof(connection));
            _isOwner = isOwner;
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
                _handle.Dispose();
            }
        }
    }
}
