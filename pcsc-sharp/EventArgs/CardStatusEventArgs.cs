using System;

namespace PCSC
{
    public class CardStatusEventArgs : EventArgs
    {
        public string ReaderName;
        public SCRState State;
        public byte[] Atr;

        public CardStatusEventArgs() { }
        public CardStatusEventArgs(string readerName, SCRState state, byte[] atr)
        {
            ReaderName = readerName;
            State = state;
            Atr = atr;
        }
    }
}