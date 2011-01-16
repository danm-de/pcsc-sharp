using System;
using System.Collections.Generic;
using System.Text;

using PCSC;

namespace GetReaderAttrib
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

            // Receive the ATR of each reader by using the GetAttrib function
            foreach (string name in readernames)
            {
                SCardReader reader = new SCardReader(ctx);

                Console.Write("Trying to connect to reader.. " + name);

                // Connect to the reader, error if no card present.
                SCardError rc = reader.Connect(
                    name,
                    SCardShareMode.Shared,
                    SCardProtocol.Any);

                if (rc == SCardError.Success)
                {
                    // Reader is now connected.
                    Console.WriteLine(" done.");

                    // receive ATR string attribute
                    byte[] atr;
                    rc = reader.GetAttrib(SCardAttr.ATRString, out atr);

                    if (rc != SCardError.Success)
                        // ATR not supported?
                        Console.WriteLine("Error by trying to receive the ATR. "
                            + SCardHelper.StringifyError(rc) + "\n");
                    else
                    {
                        Console.WriteLine("ATR: " + StringAtr(atr) + "\n");
                    }

                    // Disconnect
                    reader.Disconnect(SCardReaderDisposition.Leave);
                }
                else
                {
                    // Probably no SmartCard present.
                    Console.WriteLine(" failed. " + SCardHelper.StringifyError(rc) + "\n");
                }
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
