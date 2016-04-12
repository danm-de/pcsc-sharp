namespace PCSC.Reactive.Events
{
    public class CardInserted : MonitorCardInfoEvent
    {
        public CardInserted(string readerName, byte[] atr, SCRState state)
            : base(readerName, atr, state) {}
    }
}