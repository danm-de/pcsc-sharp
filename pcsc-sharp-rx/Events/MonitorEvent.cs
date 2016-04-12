namespace PCSC.Reactive.Events
{
    public abstract class MonitorEvent
    {
        public string ReaderName { get; }
        public byte[] Atr { get; }

        public MonitorEvent(string readerName, byte[] atr) {
            ReaderName = readerName;
            Atr = atr;
        }
    }
}