using System;
using System.Collections.Generic;
using System.Text;

using PCSC;

namespace CardStatus
{
    class Program
    {
        static void Main(string[] args)
        {
            SCardContext ctx = new SCardContext();

            ctx.Establish(SCardScope.System);

            // retrieve all reader names
            string[] readernames = ctx.GetReaders();

            if (readernames != null)
            {
                // get the card status of each reader that is currently connected
                foreach (string readername in readernames)
                {
                    SCardReader reader = new SCardReader(ctx);
                    Console.Write("Trying to connect to reader " + readername + "..");

                    SCardError serr = reader.Connect(readername,
                        SCardShareMode.Shared,
                        SCardProtocol.Any);

                    if (serr == SCardError.Success)
                    {
                        // SmartCard inserted, reader is now connected.
                        Console.WriteLine(" done.");

                        string[] tmpreadernames;
                        SCardProtocol proto;
                        SCardState state;
                        byte[] atr;

                        serr = reader.Status(
                            out tmpreadernames, // contains the reader name(s)
                            out state,          // contains the current state (flags)
                            out proto,          // contains the currently used communication protocol
                            out atr);           // contains the card ATR

                        if (serr == SCardError.Success)
                        {
                            Console.WriteLine("Connected with protocol " +
                                proto + " in state " + state);
                            if (atr != null && atr.Length > 0)
                            {
                                Console.Write("Card ATR: ");
                                foreach (byte b in atr)
                                    Console.Write("{0:X2}", b);
                                Console.WriteLine();
                            }

                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Unable to retrieve card status.\nError message: "
                                + SCardHelper.StringifyError(serr)
                                + ".\n");

                        }

                        reader.Disconnect(SCardReaderDisposition.Reset);
                    }
                    else
                    {
                        /* SmardCard not inserted or reader is reserved exclusively by
                           another application. */
                        Console.WriteLine(" failed.\nError message: "
                            + SCardHelper.StringifyError(serr)
                            + ".\n");
                    }
                }
            }
            return;
        }
    }
}
