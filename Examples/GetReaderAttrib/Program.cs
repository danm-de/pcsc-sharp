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
                using (var reader = new SCardReader(context)) {

                    if (!ConnectReader(reader, readerName)) {
                        // error while connecting ..
                        continue;
                    }

                    DisplayCardAtr(reader);
                    reader.Disconnect(SCardReaderDisposition.Leave);
                }
            }
        }

        /// <summary>
        /// Connect to reader using <paramref name="name"/>
        /// </summary>
        /// <param name="reader">Smartcard reader instance</param>
        /// <param name="name">Requested reader name</param>
        /// <returns><c>true</c> if connection attempt was successful</returns>
        private static bool ConnectReader(ISCardReader reader, string name) {
            Console.Write($"Trying to connect to reader.. {name}");
            var rc = reader.Connect(name, SCardShareMode.Shared, SCardProtocol.Any);

            if (rc == SCardError.Success) {
                Console.WriteLine(" done.");
                return true;
            }

            Console.WriteLine(" failed. No smart card present? " + SCardHelper.StringifyError(rc) + "\n");
            return false;
        }

        /// <summary>
        /// Receive and print ATR string attribute
        /// </summary>
        /// <param name="reader">Connected smartcard reader instance</param>
        private static void DisplayCardAtr(ISCardReader reader) {
            var rc = reader.GetAttrib(SCardAttribute.AtrString, out var atr);

            if (rc != SCardError.Success) {
                // ATR not supported?
                Console.WriteLine("Error by trying to receive the ATR. {0}\n", SCardHelper.StringifyError(rc));
            } else {
                Console.WriteLine("ATR: {0}\n", BitConverter.ToString(atr ?? new byte[] {}));
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
