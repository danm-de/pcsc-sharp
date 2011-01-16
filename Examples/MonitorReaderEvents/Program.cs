using System;
using System.Collections.Generic;
using System.Text;

using PCSC;

namespace MonitorReaderEvents
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleKeyInfo keyinfo;

            Console.WriteLine("This program will monitor all SmartCard readers and display all status changes.");
            Console.WriteLine("Press a key to continue.");

            keyinfo = Console.ReadKey();

            // Retrieve the names of all installed readers.
            SCardContext ctx = new SCardContext();
            ctx.Establish(SCardScope.System);
            string[] readernames = ctx.GetReaders();
            ctx.Release();

            if (readernames == null || readernames.Length == 0)
            {
                Console.WriteLine("There are currently no readers installed.");
                return;
            }

            // Create a monitor object with its own PC/SC context.
            SCardMonitor monitor = new SCardMonitor(
                new SCardContext(),
                SCardScope.System);

            // Point the callback function(s) to the static defined methods below.
            monitor.CardInserted += new CardInsertedEvent(CardInserted);
            monitor.CardRemoved += new CardRemovedEvent(CardRemoved);
            monitor.Initialized += new CardInitializedEvent(Initialized);
            monitor.StatusChanged += new StatusChangeEvent(StatusChanged);
            monitor.MonitorException += new MonitorExceptionEvent(MonitorException);

            foreach (string reader in readernames)
                Console.WriteLine("Start monitoring for reader " + reader + ".");

            monitor.Start(readernames);

            // Let the program run until the user presses a key
            keyinfo = Console.ReadKey();
            GC.KeepAlive(keyinfo);

            // Stop monitoring
            monitor.Cancel();
        }

        static void CardInserted(object sender, CardStatusEventArgs args)
        {
            SCardMonitor monitor = (SCardMonitor)sender;

            Console.WriteLine(">> CardInserted Event for reader: "
                + args.ReaderName);
            Console.WriteLine("   ATR: " + StringAtr(args.Atr));
            Console.WriteLine("   State: " + args.State + "\n");
        }

        static void CardRemoved(object sender, CardStatusEventArgs args)
        {
            SCardMonitor monitor = (SCardMonitor)sender;

            Console.WriteLine(">> CardRemoved Event for reader: "
                + args.ReaderName);
            Console.WriteLine("   ATR: " + StringAtr(args.Atr));
            Console.WriteLine("   State: " + args.State + "\n");
        }

        static void Initialized(object sender, CardStatusEventArgs args)
        {
            SCardMonitor monitor = (SCardMonitor)sender;

            Console.WriteLine(">> Initialized Event for reader: "
                + args.ReaderName);
            Console.WriteLine("   ATR: " + StringAtr(args.Atr));
            Console.WriteLine("   State: " + args.State + "\n");
        }

        static void StatusChanged(object sender, StatusChangeEventArgs args)
        {
            SCardMonitor monitor = (SCardMonitor)sender;

            Console.WriteLine(">> StatusChanged Event for reader: "
                + args.ReaderName);
            Console.WriteLine("   ATR: " + StringAtr(args.ATR));
            Console.WriteLine("   Last state: " + args.LastState
                + "\n   New state: " + args.NewState + "\n");
        }

        static void MonitorException(object sender, PCSCException ex)
        {
            Console.WriteLine("Monitor exited due an error:");
            Console.WriteLine(SCardHelper.StringifyError(ex.SCardError));
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
