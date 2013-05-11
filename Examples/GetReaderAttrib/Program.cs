using System;
using PCSC;

namespace GetReaderAttrib
{
    public class Program
    {
        public static void Main() {
            var context = new SCardContext();
            context.Establish(SCardScope.System);

            var readerNames = context.GetReaders();
            if (readerNames == null || readerNames.Length < 1) {
                Console.WriteLine("You need at least one reader in order to run this example.");
                Console.ReadKey();
                return;
            }

            // Receive the ATR of each reader by using the GetAttrib function
            foreach (var readerName in readerNames) {
                var reader = new SCardReader(context);

                Console.Write("Trying to connect to reader.. " + readerName);

                // Connect to the reader, error if no card present.
                var rc = reader.Connect(readerName, SCardShareMode.Shared, SCardProtocol.Any);

                if (rc != SCardError.Success) {
                    Console.WriteLine(" failed. No smart card present? " + SCardHelper.StringifyError(rc) + "\n");
                } else {
                    Console.WriteLine(" done.");

                    // receive ATR string attribute
                    byte[] atr;
                    rc = reader.GetAttrib(SCardAttr.ATRString, out atr);

                    if (rc != SCardError.Success) {
                        // ATR not supported?
                        Console.WriteLine("Error by trying to receive the ATR. {0}\n", SCardHelper.StringifyError(rc));
                    } else {
                        Console.WriteLine("ATR: {0}\n", BitConverter.ToString(atr ?? new byte[] {}));
                    }

                    reader.Disconnect(SCardReaderDisposition.Leave);
                }
            }

            // We MUST release here since we didn't use the 'using(..)' statement
            context.Release();
            Console.ReadKey();
        }
    }
}
