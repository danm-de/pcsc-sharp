namespace PCSC.Reactive.Events
{
    public class MonitorInitialized : MonitorCardInfoEvent
    {
        public MonitorInitialized(string readerName, byte[] atr, SCRState state) 
            : base(readerName, atr, state) {}
    }
}