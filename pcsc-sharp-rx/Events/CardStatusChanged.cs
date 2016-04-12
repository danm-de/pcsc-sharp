namespace PCSC.Reactive.Events
{
    public class CardStatusChanged : MonitorEvent
    {
        public SCRState NewState { get; }
        public SCRState PreviousState { get; }

        public CardStatusChanged(string readerName, byte[] atr, SCRState previousState, SCRState newState)
            : base(readerName, atr) 
        {
            PreviousState = previousState;
            NewState = newState;
        }
    }
}