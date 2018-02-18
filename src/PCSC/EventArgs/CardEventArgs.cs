using System;

namespace PCSC
{
    /// <summary>
    /// Smart card event.
    /// </summary>
    public abstract class CardEventArgs : EventArgs {
        /// <summary>
        /// Name of the reader that has raised the event.
        /// </summary>
        /// <remarks>A human readable string of the reader name.</remarks>
        public string ReaderName { get; private set; }
        
        /// <summary>
        /// The card's ATR (if present), otherwise <c>null</c>.
        /// </summary>
        public byte[] Atr { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        protected CardEventArgs() {}

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="readerName">Name of the reader that has raised the event.</param>
        /// <param name="atr">The card's ATR (if present), otherwise <c>null</c>.</param>
        protected CardEventArgs(string readerName, byte[] atr) {
            ReaderName = readerName;
            Atr = atr;
        }
    }
}