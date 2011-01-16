using System;
using System.Collections.Generic;
using System.Text;
using PCSC;
using PCSC.Iso7816;

namespace SmartCardTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Establish PC/SC context
            SCardContext ctx = new SCardContext();
            ctx.Establish(SCardScope.System);

            // Create a reader object
            SCardReader reader = new SCardReader(ctx);

            // Use the first reader that is found
            string firstreader = ctx.GetReaders()[0];

            // Connect to the card
            IsoCard card = new IsoCard(reader);
            card.Connect(firstreader, SCardShareMode.Shared, SCardProtocol.Any);

            // Build a ATR fetch case
            CommandApdu apdu = card.ConstructCommandApdu(
                IsoCase.Case2Short);

            apdu.CLA = 0x00; // Class
            apdu.INS = 0x84; // Instruction: GET CHALLENGE 
            apdu.P1 = 0x00;  // Parameter 1
            apdu.P2 = 0x00;  // Parameter 2
            apdu.Le = 0x08;  // Expected length of the returned data

            // Transmit the Command APDU to the card and receive the response
            Response resp = card.Transmit(apdu);

            // Show SW1SW2 from the last response packet (if more than one has been received).
            Console.WriteLine("SW1: {0:X2} SW2: {1:X2}", resp.SW1, resp.SW2);

            byte[] data;

            // First test - get the data from all response APDUs
            data = resp.GetData();
            if (data != null)
            {
                Console.Write("CHALLENGE:");

                foreach (byte b in data)
                    Console.Write(" {0:X2}", b);
                Console.WriteLine();
            }

            // Second test - get the data from each response APDU.
            int i = 0;
            foreach (ResponseApdu respApdu in resp.ResponseApduList)
            {
                data = respApdu.GetData();

                if (data != null)
                {
                    Console.Write("APDU ({0}), DATA:", i);
                    foreach (byte b in data)
                        Console.Write(" {0:X2}", b);
                    Console.WriteLine();
                    i++;
                }
            }
            return;
        }
    }
}
