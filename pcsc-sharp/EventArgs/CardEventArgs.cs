using System;

namespace PCSC
{
    public abstract class CardEventArgs : EventArgs {
        public string ReaderName { get; private set; }
        public byte[] Atr { get; private set; }

        public CardEventArgs() {}
        public CardEventArgs(string readerName, byte[] atr) {
            ReaderName = readerName;
            Atr = atr;
        }
    }
}