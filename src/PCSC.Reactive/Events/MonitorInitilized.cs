namespace PCSC.Reactive.Events
{
    /// <summary>
    /// The smart card monitor has been initialized
    /// </summary>
    public class MonitorInitialized : MonitorCardInfoEvent
    {
        /// <summary>
        /// Creates a new MonitorInitialized instance 
        /// </summary>
        /// <param name="readerName">Name of the smard card reader</param>
        /// <param name="atr">The card's ATR</param>
        /// <param name="state">The reader's state</param>
        public MonitorInitialized(string readerName, byte[] atr, SCRState state) 
            : base(readerName, atr, state) {}
    }
}