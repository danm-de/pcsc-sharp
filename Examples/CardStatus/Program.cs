using System;
using PCSC;

namespace CardStatus
{
    public class Program
    {
        static void Main()
        {
            using (var context = new SCardContext()) {

                context.Establish(SCardScope.System);

                // retrieve all reader names
                var readerNames = context.GetReaders();

                if (readerNames == null) {
                    Console.WriteLine("No readers found.");
                    Console.ReadKey();
                    return;
                }

                // get the card status of each reader that is currently connected
                foreach (var readerName in readerNames) {
                    using (var reader = new SCardReader(context)) {
                        Console.WriteLine("Trying to connect to reader {0}.",  readerName);

                        var sc = reader.Connect(readerName, SCardShareMode.Shared, SCardProtocol.Any);
                        if (sc == SCardError.Success) {
                            DisplayReaderStatus(reader);
                        } else {
                            Console.WriteLine("No card inserted or reader is reserved exclusively by another application.");
                            Console.WriteLine("Error message: {0}\n", SCardHelper.StringifyError(sc));
                        }
                    }
                }

                Console.ReadKey();
            }
        }

        private static void DisplayReaderStatus(ISCardReader reader) {
            string[] readerNames;
            SCardProtocol proto;
            SCardState state;
            byte[] atr;

            var sc = reader.Status(
                out readerNames,    // contains the reader name(s)
                out state,          // contains the current state (flags)
                out proto,          // contains the currently used communication protocol
                out atr);           // contains the ATR

            if (sc == SCardError.Success) {
                Console.WriteLine("Connected with protocol {0} in state {1}", proto, state);
                DisplayCardAtr(atr);
                Console.WriteLine();
            } else {
                Console.WriteLine("Unable to retrieve card status.\nError message: {0}", 
                    SCardHelper.StringifyError(sc));
            }
        }

        private static void DisplayCardAtr(byte[] atr) {
            if (atr == null || atr.Length <= 0) {
                return;
            }

            Console.WriteLine("Card ATR: {0}", BitConverter.ToString(atr));
        }
    }
}
