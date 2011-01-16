using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using PCSC;

namespace ReaderStatus
{
    class Program
    {
        static void Main(string[] args)
        {
            // Retrieve the names of all installed readers.
            SCardContext ctx = new SCardContext();
            ctx.Establish(SCardScope.System);
            string[] readernames = ctx.GetReaders();

            /* Get the current status of each reader in "readernames".
             */
            SCardReaderState[] states = ctx.GetReaderStatus(readernames);

            foreach (SCardReaderState state in states)
            {
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                Console.WriteLine("Reader: " + state.ReaderName);
                Console.WriteLine("CurrentState: " + state.CurrentState
                    + " EventState: " + state.EventState + "\n"
                    + "CurrentStateValue: " + state.CurrentStateValue
                    + " EventStateValue: " + state.EventStateValue);
                Console.WriteLine("UserData: " + state.UserData.ToString()
                    + " CardChangeEventCnt: " + state.CardChangeEventCnt);
                Console.WriteLine("ATR: " + StringAtr(state.ATR));
            }

            ctx.Release();
            return;
        }

        /// <summary>
        /// Helper function that translates a byte array into an hex-encoded ATR string.
        /// </summary>
        /// <param name="atr">Contains the SmartCard ATR.</param>
        /// <returns></returns>
        static string StringAtr(byte[] atr)
        {
            if (atr == null)
                return null;

            StringBuilder sb = new StringBuilder();
            foreach (byte b in atr)
                sb.AppendFormat("{0:X2}", b);

            return sb.ToString();
        }
    }
}
