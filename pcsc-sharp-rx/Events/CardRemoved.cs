namespace PCSC.Reactive.Events
{
    public class CardRemoved : MonitorCardInfoEvent
    {
        public CardRemoved(string readerName, byte[] atr, SCRState state)
            : base(readerName, atr, state) {}
    }
}