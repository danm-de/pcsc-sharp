namespace PCSC.Reactive.Events
{
    /// <summary>
    /// Smart card monitor event
    /// </summary>
    public abstract class MonitorEvent
    {
        /// <summary>
        /// Name of the smard card reader
        /// </summary>
        public string ReaderName { get; }

        /// <summary>
        /// The card's ATR
        /// </summary>
        public byte[] Atr { get; }

        /// <summary>
        /// Creates a new monitor event instance
        /// </summary>
        /// <param name="readerName">Name of the smard card reader</param>
        /// <param name="atr">The card's ATR</param>
        protected MonitorEvent(string readerName, byte[] atr) {
            ReaderName = readerName;
            Atr = atr;
        }
    }
}
