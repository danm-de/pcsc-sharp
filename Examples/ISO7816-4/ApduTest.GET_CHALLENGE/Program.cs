using System;
using System.Collections.Generic;
using PCSC;
using PCSC.Iso7816;

namespace APDUTest
{
    public class Program
    {
        private static void Main() {
            var contextFactory = ContextFactory.Instance;
            using (var ctx = contextFactory.Establish(SCardScope.System)) {
                var readerNames = ctx.GetReaders();
                if (NoReaderFound(readerNames)) {
                    Console.WriteLine("You need at least one reader in order to run this example.");
                    Console.ReadKey();
                    return;
                }

                var name = ChooseReader(readerNames);
                if (name == null) {
                    return;
                }

                using (var isoReader = new IsoReader(
                    context: ctx,
                    readerName: name,
                    mode: SCardShareMode.Shared,
                    protocol: SCardProtocol.Any,
                    releaseContextOnDispose: false)) {

                    // Build a GET CHALLENGE command 
                    var apdu = new CommandApdu(IsoCase.Case2Short, isoReader.ActiveProtocol) {
                        CLA = 0x00, // Class
                        Instruction = InstructionCode.GetChallenge,
                        P1 = 0x00, // Parameter 1
                        P2 = 0x00, // Parameter 2
                        Le = 0x08 // Expected length of the returned data
                    };

                    Console.WriteLine("Send APDU with \"GET CHALLENGE\" command: {0}",
                        BitConverter.ToString(apdu.ToArray()));

                    var response = isoReader.Transmit(apdu);

                    Console.WriteLine("SW1 SW2 = {0:X2} {1:X2}", 
                        response.SW1, response.SW2);

                    if (!response.HasData) {
                        Console.WriteLine("No data. (Card does not understand \"GET CHALLENGE\")");
                    } else {
                        var data = response.GetData();
                        Console.WriteLine("Challenge: {0}", BitConverter.ToString(data));
                    }
                }
            }
            Console.ReadKey();
        }

        private static string ChooseReader(IList<string> readerNames) {
            // Show available readers.
            Console.WriteLine("Available readers: ");
            for (var i = 0; i < readerNames.Count; i++) {
                Console.WriteLine("[" + i + "] " + readerNames[i]);
            }

            // Ask the user which one to choose.
            Console.Write("Which reader has an inserted card that supports the GET CHALLENGE command? ");
            var line = Console.ReadLine();
            int choice;

            if (int.TryParse(line, out choice) && (choice >= 0) && (choice <= readerNames.Count)) {
                return readerNames[choice];
            }

            Console.WriteLine("An invalid number has been entered.");
            Console.ReadKey();
            return null;
        }

        private static bool NoReaderFound(ICollection<string> readerNames) {
            return readerNames == null || readerNames.Count < 1;
        }
    }
}