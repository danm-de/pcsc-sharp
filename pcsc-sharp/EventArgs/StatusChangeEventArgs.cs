using System;

namespace PCSC
{
    public class StatusChangeEventArgs : EventArgs
    {
        public string ReaderName;
        public SCRState LastState;
        public SCRState NewState;
        public byte[] ATR;

        public StatusChangeEventArgs() { }
        public StatusChangeEventArgs(string readerName, SCRState lastState, SCRState newState, byte[] atr)
        {
            ReaderName = readerName;
            LastState = lastState;
            NewState = newState;
            ATR = atr;
        }
    }
}