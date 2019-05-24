using System;
using System.Linq;
using PCSC;

namespace ConnectedReaderStatus {
    class Program {
        static void Main(string[] args) {
            using (var ctx = ContextFactory.Instance.Establish(SCardScope.User)) {
                var firstReader = ctx
                    .GetReaders()
                    .FirstOrDefault();

                if (firstReader == null) {
                    Console.WriteLine("No reader connected.");
                    return;
                }

                using (var reader = ctx.ConnectReader(firstReader, SCardShareMode.Direct, SCardProtocol.Unset)) {
                    var status = reader.GetStatus();

                    Console.WriteLine($"Reader names: {string.Join(", ", status.GetReaderNames())}");
                    Console.WriteLine($"Protocol: {status.Protocol}");
                    Console.WriteLine($"State: {status.State}");
                    Console.WriteLine($"ATR: {BitConverter.ToString(status.GetAtr() ?? new byte[0])}");
                }

                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }
    }
}
