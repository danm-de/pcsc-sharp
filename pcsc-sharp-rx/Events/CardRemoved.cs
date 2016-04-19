namespace PCSC.Reactive.Events
{
    /// <summary>
    /// A card has been removed
    /// </summary>
    public class CardRemoved : MonitorCardInfoEvent
    {
        /// <summary>
        /// Creates a new CardRemoved instance
        /// </summary>
        /// <param name="readerName">Name of the smard card reader</param>
        /// <param name="atr">The card's ATR</param>
        /// <param name="state">The reader's state</param>
        public CardRemoved(string readerName, byte[] atr, SCRState state)
            : base(readerName, atr, state) {}
    }
}