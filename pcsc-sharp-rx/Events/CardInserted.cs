namespace PCSC.Reactive.Events
{
    /// <summary>
    /// A new card has been inserted
    /// </summary>
    public class CardInserted : MonitorCardInfoEvent
    {
        /// <summary>
        /// Creates an CardInserted instance
        /// </summary>
        /// <param name="readerName">Name of the reader</param>
        /// <param name="atr">The card's ATR</param>
        /// <param name="state">The reader's state</param>
        public CardInserted(string readerName, byte[] atr, SCRState state)
            : base(readerName, atr, state) {}
    }
}