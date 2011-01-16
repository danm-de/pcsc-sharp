using System;
using System.Collections.Generic;
using System.Text;

using PCSC;

namespace Transmit
{
    class Program
    {
        static void Main(string[] args)
        {
            SCardContext ctx = new SCardContext();
            ctx.Establish(SCardScope.System);

            string[] readernames = ctx.GetReaders();
            if (readernames == null || readernames.Length < 1)
                throw new Exception("You need at least one reader in order to run this example.");

            // Show available readers.
            Console.WriteLine("Available readers: ");
            for (int i = 0; i < readernames.Length; i++)
                Console.WriteLine("[" + i + "] " + readernames[i]);

            int num = 0;

            if (readernames.Length > 1)
            {
                // Ask the user which one to choose.
                Console.Write("Which reader is an RFID reader? ");
                string relin = Console.ReadLine();
                if (!(int.TryParse(relin, out num))
                    || num < 0
                    || num > readernames.Length)
                {
                    Console.WriteLine("An invalid number has been entered. Exiting.");
                    return;
                }
            }

            string readername = readernames[num];

            SCardReader RFIDReader = new SCardReader(ctx);
            SCardError rc = RFIDReader.Connect(
                readername,
                SCardShareMode.Shared,
                SCardProtocol.Any);

            if (rc != SCardError.Success)
            {
                Console.WriteLine("Unable to connect to RFID card / chip. Error: " +
                    SCardHelper.StringifyError(rc));
                return;
            }

            // prepare APDU
            byte[] ucByteSend = new byte[] 
            {
                0xFF,   // the instruction class
                0xCA,   // the instruction code 
                0x00,   // parameter to the instruction
                0x00,   // parameter to the instruction
                0x00    // size of I/O transfer
            };
            byte[] ucByteReceive = new byte[10];

            Console.Out.WriteLine("Retrieving the UID .... ");

            rc = RFIDReader.BeginTransaction();
            if (rc != SCardError.Success)
                throw new Exception("Could not begin transaction.");

            SCardPCI ioreq = new SCardPCI();    /* creates an empty object (null).
                                                 * IO returned protocol control information.
                                                 */
            IntPtr sendPci = SCardPCI.GetPci(RFIDReader.ActiveProtocol);
            rc = RFIDReader.Transmit(
                sendPci,    /* Protocol control information, T0, T1 and Raw
                             * are global defined protocol header structures.
                             */
                ucByteSend, /* the actual data to be written to the card */
                ioreq,      /* The returned protocol control information */
                ref ucByteReceive);

            if (rc == SCardError.Success)
            {
                Console.Write("Uid: ");
                for (int i = 0; i < (ucByteReceive.Length); i++)
                    Console.Write("{0:X2} ", ucByteReceive[i]);
                Console.WriteLine("");
            }
            else
            {
                Console.WriteLine("Error: " + SCardHelper.StringifyError(rc));
            }

            RFIDReader.EndTransaction(SCardReaderDisposition.Leave);
            RFIDReader.Disconnect(SCardReaderDisposition.Reset);

            return;
        }
    }
}
