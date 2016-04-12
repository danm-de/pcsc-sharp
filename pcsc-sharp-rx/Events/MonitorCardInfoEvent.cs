namespace PCSC.Reactive.Events
{
    public class MonitorCardInfoEvent : MonitorEvent
    {
        public SCRState State { get; }

        public MonitorCardInfoEvent(string readerName, byte[] atr, SCRState state)
            : base(readerName, atr) {
            State = state;
        }
    }
}