using System;
using System.Collections.Generic;
using PCSC;

namespace GetReaderAttrib
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

                DisplayAtrs(context, readerNames);
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Receive the ATR of each reader in <paramref name="readerNames"/> by using the GetAttrib function
        /// </summary>
        /// <param name="context">Connection context</param>
        /// <param name="readerNames">Readers from which the ATR should be requested</param>
        private static void DisplayAtrs(ISCardContext context, IEnumerable<string> readerNames) {
            foreach (var readerName in readerNames) {
                try {
                    using (var reader = context.ConnectReader(readerName, SCardShareMode.Shared, SCardProtocol.Any)) {
                        DisplayAtr(reader);
                    }
                } catch (Exception exception) {
                    Console.WriteLine("Could not connect to reader {0}. No smart card present? ({1})", readerName,
                        exception.GetType());
                }
            }
        }

        /// <summary>
        /// Receive and print ATR string attribute
        /// </summary>
        /// <param name="reader">Connected smartcard reader instance</param>
        private static void DisplayAtr(ICardReader reader) {
            try {
                var atr = reader.GetAttrib(SCardAttribute.AtrString);
                Console.WriteLine("Reader: {0}, ATR: {1}", 
                    reader.Name,
                    BitConverter.ToString(atr ?? new byte[] { }));
            } catch (Exception exception) {
                Console.WriteLine("Reader: {0}, Error by trying to receive the ATR. {1} ({2})\n", 
                    reader.Name,
                    exception.Message,
                    exception.GetType());
            }
        }

        /// <summary>
        /// Checks if smartcard readers are available
        /// </summary>
        /// <param name="readerNames">Collection of reader names</param>
        /// <returns><c>true</c> if the supplied collection of <paramref name="readerNames"/> does not contain any reader name.</returns>
        private static bool NoReaderFound(ICollection<string> readerNames) {
            return readerNames == null || readerNames.Count < 1;
        }
    }
}
