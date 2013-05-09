using System;
using PCSC;
using PCSC.Iso7816;

namespace ApduTest
{
    public class Program
    {
        private static void Main() {
            // Establish Smartcard context
            using (var context = new SCardContext()) {
                context.Establish(SCardScope.System);

                var readernames = context.GetReaders();
                if (readernames == null || readernames.Length < 1) {
                    throw new Exception("You need at least one reader in order to run this example.");
                }

                // we will use the first reader for the transmit test.
                var readerName = readernames[0];

                using (var isocard = new IsoCard(new SCardReader(context), readerName, SCardShareMode.Shared, SCardProtocol.Any)) {
                    
                    // Build a GET CHALLENGE command
                    var apdu = new CommandApdu(IsoCase.Case2Short, isocard.ActiveProtocol) {
                        CLA = 0x00, // Class
                        INS = 0x84, // Instruction: GET CHALLENGE 
                        P1 = 0x00,  // Parameter 1
                        P2 = 0x00,  // Parameter 2
                        Le = 0x08   // Expected length of the returned data
                    };

                    var response = isocard.Transmit(apdu);

                    Console.WriteLine("SW1 SW2 = {0:X2} {1:X2}", response.SW1, response.SW2);

                    if (!response.HasData) {
                        Console.WriteLine("No data.");
                    } else {
                        var data = response.GetData();
                        Console.Write("Data: ");
                        Console.Write(BitConverter.ToString(data));
                        Console.WriteLine("");
                    }
                }
            }
        }
    }
}
