using System;
using System.Collections.Generic;
using System.Text;

using PCSC;
using PCSC.Iso7816;

namespace ApduTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Establish Smartcard context
            SCardContext ctx = new SCardContext();
            ctx.Establish(SCardScope.System);

            string[] readernames = ctx.GetReaders();
            if (readernames == null || readernames.Length < 1)
                throw new Exception("You need at least one reader in order to run this example.");

            // we will use the first reader for the transmit test.
            string readername = readernames[0];

            SCardReader reader = new SCardReader(ctx);
            SCardError rc = reader.Connect(
                readername,
                SCardShareMode.Shared,
                SCardProtocol.Any);
            if (rc != SCardError.Success)
            {
                Console.WriteLine("Could not connect to card in reader " + readername + "\n"
                    + "Error: " + SCardHelper.StringifyError(rc));
                return;
            }

            // Build a GET CHALLENGE command
            CommandApdu apdu = new CommandApdu(
                IsoCase.Case2Short,
                reader.ActiveProtocol);

            apdu.CLA = 0x00; // Class
            apdu.INS = 0x84; // Instruction: GET CHALLENGE 
            apdu.P1 = 0x00;  // Parameter 1
            apdu.P2 = 0x00;  // Parameter 2
            apdu.Le = 0x08;  // Expected length of the returned data
            
            // convert the APDU object into an array of bytes
            byte[] cmd = apdu.ToArray();
            // prepare a buffer for response APDU -> LE + 2 bytes (SW1 SW2)
            byte[] outbuf = new byte[apdu.ExpectedResponseLength]; 

            rc = reader.Transmit(
                cmd,
                ref outbuf);

            if (rc == SCardError.Success)
            {
                Console.WriteLine("Ok.");

                if (outbuf != null)
                {
                    ResponseApdu response = new ResponseApdu(outbuf, apdu.Case, apdu.Protocol);
                    if (response.IsValid)
                    {
                        Console.WriteLine("SW1 SW2 = {0:X2} {1:X2}", response.SW1, response.SW2);
                        if (response.HasData)
                        {
                            Console.Write("Data: ");
                            for (int i = 0; i < (response.DataSize); i++)
                                Console.Write("{0:X2} ", response.FullApdu[i]);
                            Console.WriteLine("");
                        }
                    }
                }
            }
            else
            {
                // Error
                Console.WriteLine(SCardHelper.StringifyError(rc));
            }

            return;

        }
    }
}
