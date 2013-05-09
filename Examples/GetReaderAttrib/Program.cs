using System;
using PCSC;

namespace GetReaderAttrib
{
    public class Program
    {
        private static void Main() {
            using (var context = new SCardContext()) {
                context.Establish(SCardScope.System);

                var readerNames = context.GetReaders();
                if (readerNames == null || readerNames.Length < 1) {
                    throw new Exception("You need at least one reader in order to run this example.");
                }

                // Receive the ATR of each reader by using the GetAttrib function
                foreach (var readerName in readerNames) {
                    var reader = new SCardReader(context);

                    Console.Write("Trying to connect to reader.. " + readerName);

                    // Connect to the reader, error if no card present.
                    var rc = reader.Connect(
                        readerName,
                        SCardShareMode.Shared,
                        SCardProtocol.Any);

                    if (rc == SCardError.Success) {
                        // Reader is now connected.
                        Console.WriteLine(" done.");

                        // receive ATR string attribute
                        byte[] atr;
                        rc = reader.GetAttrib(SCardAttr.ATRString, out atr);

                        if (rc != SCardError.Success)
                            // ATR not supported?
                            Console.WriteLine("Error by trying to receive the ATR. {0}\n", SCardHelper.StringifyError(rc));
                        else {
                            Console.WriteLine("ATR: {0}\n", StringAtr(atr));
                        }

                        reader.Disconnect(SCardReaderDisposition.Leave);
                    } else {
                        // Probably no SmartCard present.
                        Console.WriteLine(" failed. " + SCardHelper.StringifyError(rc) + "\n");
                    }
                }

                context.Release();
            }
        }

        /// <summary>
        /// Helper function that translates a byte array into an hex-encoded ATR string.
        /// </summary>
        /// <param name="atr">Contains the SmartCard ATR.</param>
        /// <returns></returns>
        static string StringAtr(byte[] atr) {
            return atr == null
                ? null
                : BitConverter.ToString(atr);
        }
    }
}
