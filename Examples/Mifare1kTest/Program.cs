using System;
using PCSC;
using PCSC.Iso7816;

namespace Mifare1kTest
{
    class Program
    {
        private static readonly byte[] _data_to_write = { 0x0F, 0x0E, 0x0D, 0x0C, 0x0B, 0x0A, 0x09, 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01, 0x00};
        const byte MSB = 0x00;
        const byte LSB = 0x08;

        static void Main() {
            // Establish Smartcard context
            using (var context = new SCardContext()) {
                context.Establish(SCardScope.System);

                var readerNames = context.GetReaders();
                if (readerNames == null || readerNames.Length < 1) {
                    Console.WriteLine("You need at least one reader in order to run this example.");
                    Console.ReadKey();
                    return;
                }

                var readerName = ChooseReader(readerNames);
                if (readerName == null) {
                    return;
                }

                using (var isoReader = new IsoReader(context, readerName, SCardShareMode.Shared, SCardProtocol.Any, false)) {
                    var card = new MifareCard(isoReader);

                    var loadKeySuccessful = card.LoadKey(
                        KeyStructure.NonVolatileMemory, 
                        0x00, // first key slot
                        new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF } // key
                    );

                    if (!loadKeySuccessful) {
                        throw new Exception("LOAD KEY failed.");
                    }

                    var authSuccessful = card.Authenticate(MSB, LSB, KeyType.KeyA, 0x00);
                    if (!authSuccessful) {
                        throw new Exception("AUTHENTICATE failed.");
                    }

                    var result = card.ReadBinary(MSB, LSB, 16);
                    Console.WriteLine("Result (before BINARY UPDATE): {0}", 
                        (result != null) 
                        ? BitConverter.ToString(result) 
                        : null);

                    var updateSuccessful = card.UpdateBinary(MSB, LSB, _data_to_write);

                    if (!updateSuccessful) {
                        throw new Exception("UPDATE BINARY failed.");
                    }

                    result = card.ReadBinary(MSB, LSB, 16);
                    Console.WriteLine("Result (after BINARY UPDATE): {0}", 
                        (result != null) 
                        ? BitConverter.ToString(result) 
                        : null);
                }
            }
            Console.ReadKey();
        }

        private static string ChooseReader(string[] readerNames) {
            Console.WriteLine(new string('=', 79));
            Console.WriteLine("WARNING!! This will overwrite data in MSB {0:X2} LSB {1:X2} using the default key.",MSB, LSB);
            Console.WriteLine(new string('=', 79));

            // Show available readers.
            Console.WriteLine("Available readers: ");
            for (var i = 0; i < readerNames.Length; i++) {
                Console.WriteLine("[" + i + "] " + readerNames[i]);
            }

            // Ask the user which one to choose.
            Console.Write("Which reader has an inserted Mifare 1k/4k card? ");
            
            var line = Console.ReadLine();
            int choice;

            if (int.TryParse(line, out choice) && (choice >= 0) && (choice <= readerNames.Length)) {
                return readerNames[choice];
            }

            Console.WriteLine("An invalid number has been entered.");
            Console.ReadKey();
            return null;
        }
    }
}
