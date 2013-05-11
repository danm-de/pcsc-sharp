namespace PCSC
{
    public class CardStatusEventArgs : CardEventArgs
    {
        public SCRState State { get; private set; }

        public CardStatusEventArgs() { }
        public CardStatusEventArgs(string readerName, SCRState state, byte[] atr)
            :base(readerName, atr)
        {
            State = state;
        }
    }
}