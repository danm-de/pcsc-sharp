namespace PCSC
{
    public class StatusChangeEventArgs : CardEventArgs
    {
        public SCRState LastState { get; private set; }
        public SCRState NewState { get; private set; }
        
        public StatusChangeEventArgs() { }
        public StatusChangeEventArgs(string readerName, SCRState lastState, SCRState newState, byte[] atr)
            :base(readerName, atr)
        {
            LastState = lastState;
            NewState = newState;
        }
    }
}