using System;
using System.Collections.Generic;
using PCSC;
using PCSC.Iso7816;

namespace Transmit
{
    public class Program
    {
        public static void Main() {
            var contextFactory = ContextFactory.Instance;
            using (var context = contextFactory.Establish(SCardScope.System)) {
                var readerNames = context.GetReaders();
                if (NoReaderFound(readerNames)) {
                    Console.WriteLine("You need at least one reader in order to run this example.");
                    Console.ReadKey();
                    return;
                }

                var readerName = ChooseRfidReader(readerNames);
                if (readerName == null) {
                    return;
                }

                // 'using' statement to make sure the reader will be disposed (disconnected) on exit
                using (var rfidReader = context.ConnectReader(readerName, SCardShareMode.Shared, SCardProtocol.Any)) {
                    var apdu = new CommandApdu(IsoCase.Case2Short, rfidReader.Protocol) {
                        CLA = 0xFF,
                        Instruction = InstructionCode.GetData,
                        P1 = 0x00,
                        P2 = 0x00,
                        Le = 0 // We don't know the ID tag size
                    };

                    using (rfidReader.Transaction(SCardReaderDisposition.Leave)) {
                        Console.WriteLine("Retrieving the UID .... ");

                        var sendPci = SCardPCI.GetPci(rfidReader.Protocol);
                        var receivePci = new SCardPCI(); // IO returned protocol control information.

                        var receiveBuffer = new byte[256];
                        var command = apdu.ToArray();

                        var bytesReceived = rfidReader.Transmit(
                            sendPci, // Protocol Control Information (T0, T1 or Raw)
                            command, // command APDU
                            command.Length,
                            receivePci, // returning Protocol Control Information
                            receiveBuffer,
                            receiveBuffer.Length); // data buffer

                        var responseApdu =
                            new ResponseApdu(receiveBuffer, bytesReceived, IsoCase.Case2Short, rfidReader.Protocol);
                        Console.Write("SW1: {0:X2}, SW2: {1:X2}\nUid: {2}",
                            responseApdu.SW1,
                            responseApdu.SW2,
                            responseApdu.HasData ? BitConverter.ToString(responseApdu.GetData()) : "No uid received");
                    }

                    Console.ReadKey();
                }
            }
        }

        private static string ChooseRfidReader(IList<string> readerNames) {
            // Show available readers.
            Console.WriteLine("Available readers: ");
            for (var i = 0; i < readerNames.Count; i++) {
                Console.WriteLine($"[{i}] {readerNames[i]}");
            }

            // Ask the user which one to choose.
            Console.Write("Which reader is an RFID reader? ");
            var line = Console.ReadLine();

            if (int.TryParse(line, out var choice) && choice >= 0 && (choice <= readerNames.Count)) {
                return readerNames[choice];
            }

            Console.WriteLine("An invalid number has been entered.");
            Console.ReadKey();
            return null;
        }

        private static bool NoReaderFound(ICollection<string> readerNames) =>
            readerNames == null || readerNames.Count < 1;
    }
}
