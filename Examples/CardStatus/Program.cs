using System;
using System.Collections.Generic;
using PCSC;
using PCSC.Context;
using PCSC.Reader;
using PCSC.Utils;

namespace CardStatus
{
    public class Program
    {
        public static void Main() {
            var contextFactory = ContextFactory.Instance;

            using (var context = contextFactory.Establish(SCardScope.System)) {
                var readerNames = context.GetReaders();

                if (NoReaderFound(readerNames)) {
                    Console.WriteLine("No readers found.");
                } else {
                    DisplayReaderStatus(context, readerNames);
                }

                Console.ReadKey();
            }
        }

        /// <summary>
        /// Displays the card status of each reader in <paramref name="readerNames"/>
        /// </summary>
        /// <param name="context">Smartcard context to connect</param>
        /// <param name="readerNames">Smartcard readers</param>
        private static void DisplayReaderStatus(ISCardContext context, IEnumerable<string> readerNames) {
            foreach (var readerName in readerNames) {
                using (var reader = new SCardReader(context)) {
                    Console.WriteLine("Trying to connect to reader {0}.", readerName);

                    var sc = reader.Connect(readerName, SCardShareMode.Shared, SCardProtocol.Any);
                    if (sc != SCardError.Success) {
                        Console.WriteLine("No card inserted or reader is reserved exclusively by another application.");
                        Console.WriteLine("Error message: {0}\n", SCardHelper.StringifyError(sc));
                        continue;
                    }

                    PrintReaderStatus(reader);
                    Console.WriteLine();
                    reader.Disconnect(SCardReaderDisposition.Reset);
                }
            }
        }

        /// <summary>
        /// Queries the reader's status and prints it out
        /// </summary>
        /// <param name="reader">Connected reader</param>
        private static void PrintReaderStatus(ISCardReader reader) {
            var sc = reader.Status(
                out var readerNames, // contains the reader name(s)
                out var state, // contains the current state (flags)
                out var proto, // contains the currently used communication protocol
                out var atr); // contains the ATR

            if (sc != SCardError.Success) {
                Console.WriteLine("Unable to retrieve card status.\nError message: {0}",
                    SCardHelper.StringifyError(sc));
                return;
            }

            Console.WriteLine("Connected with protocol {0} in state {1}", proto, state);
            PrintCardAtr(atr);
        }

        /// <summary>
        /// Prints the smart cards ATR as hex string
        /// </summary>
        /// <param name="atr">ATR bytes</param>
        private static void PrintCardAtr(byte[] atr) {
            if (atr == null || atr.Length <= 0) {
                return;
            }

            Console.WriteLine("Card ATR: {0}", BitConverter.ToString(atr));
        }

        /// <summary>
        /// Returns <c>true</c> if the supplied collection <paramref name="readerNames"/> does not contain any reader name.
        /// </summary>
        /// <param name="readerNames">Collection of smartcard reader names</param>
        /// <returns><c>true</c> if no reader found</returns>
        private static bool NoReaderFound(ICollection<string> readerNames) {
            return readerNames == null || readerNames.Count < 1;
        }
    }
}
