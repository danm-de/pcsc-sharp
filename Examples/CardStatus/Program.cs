using System;
using System.Collections.Generic;
using System.Linq;
using PCSC;

namespace CardStatus {
    public class Program {
        public static void Main() {
            var contextFactory = ContextFactory.Instance;

            using (var context = contextFactory.Establish(SCardScope.System)) {
                var readerNames = context.GetReaders();

                if (IsEmpty(readerNames)) {
                    Console.WriteLine("No readers found.");
                    Console.ReadKey();
                    return;
                }

                DisplayReaderStatus(context, readerNames);

                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Displays the card status of each reader in <paramref name="readerNames"/>
        /// </summary>
        /// <param name="context">Smart-card context to connect</param>
        /// <param name="readerNames">Smart-card readers</param>
        private static void DisplayReaderStatus(ISCardContext context, IEnumerable<string> readerNames) {
            foreach (var readerName in readerNames) {
                try {
                    using (var reader = context.ConnectReader(readerName, SCardShareMode.Shared, SCardProtocol.Any)) {
                        PrintReaderStatus(reader);
                        Console.WriteLine();
                    }
                } catch (Exception exception) {
                    Console.Error.WriteLine(
                        "No card inserted or reader '{0}' is reserved exclusively by another application.", readerName);
                    Console.Error.WriteLine("Error message: {0} ({1})\n", exception.Message, exception.GetType());
                }
            }
        }

        /// <summary>
        /// Prints the reader's status
        /// </summary>
        /// <param name="reader">Connected reader</param>
        private static void PrintReaderStatus(ICardReader reader) {
            try {
                var status = reader.GetStatus();
                Console.WriteLine("Reader {0} connected with protocol {1} in state {2}",
                    status.GetReaderNames().FirstOrDefault(),
                    status.Protocol,
                    status.State);
                PrintCardAtr(status.GetAtr());
            } catch (Exception exception) {
                Console.Error.WriteLine("Unable to retrieve card status.\nError message: {0} ({1}", exception,
                    exception.GetType());
            }
        }

        /// <summary>
        /// Prints the smart-card's ATR as hex string
        /// </summary>
        /// <param name="atr">ATR bytes</param>
        private static void PrintCardAtr(byte[] atr) {
            if (atr == null || atr.Length <= 0) {
                return;
            }

            Console.WriteLine("Card ATR: {0}", BitConverter.ToString(atr));
        }

        /// <summary>
        /// Returns <c>true</c> if the supplied collection <paramref name="readerNames"/> does not contain any reader.
        /// </summary>
        /// <param name="readerNames">Collection of smart-card reader names</param>
        /// <returns><c>true</c> if no reader found</returns>
        private static bool IsEmpty(ICollection<string> readerNames) => readerNames == null || readerNames.Count < 1;
    }
}
