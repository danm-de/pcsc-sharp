using System.Linq;

namespace PCSC
{
    /// <summary>
    /// Holds information about the reader status.
    /// </summary>
    public sealed class ReaderStatus
    {
        private readonly byte[] _atr;
        private readonly string[] _readerNames;

        /// <summary>
        /// A bit mask that represents the reader status
        /// </summary>
        public SCardState State { get; }

        /// <summary>
        /// The reader's currently used protocol.
        /// </summary>
        public SCardProtocol Protocol { get; }

        /// <summary>
        /// A list of the reader's friendly names
        /// </summary>
        public string[] GetReaderNames() => _readerNames?.ToArray(); // return copy or NULL

        /// <summary>
        /// Gets the card's ATR.
        /// </summary>
        /// <returns>A byte array containing the ATR or <c>null</c> if no card connected.</returns>
        public byte[] GetAtr() => _atr?.ToArray(); // return copy or NULL

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="readerNames">The reader's friendly names</param>
        /// <param name="state">A bit mask that represents the reader status</param>
        /// <param name="protocol">The reader's currently used protocol.</param>
        /// <param name="atr">The card's ATR if available</param>
        public ReaderStatus(string[] readerNames, SCardState state, SCardProtocol protocol, byte[] atr = null) {
            State = state;
            Protocol = protocol;
            _readerNames = readerNames;
            _atr = atr;
        }
    }
}
