using System;
using System.Collections.Generic;
using System.Linq;
using PCSC;
using PCSC.Exceptions;

namespace MonitorDeviceEventsNative {
    public class Program {
        public static void Main() {
            Console.WriteLine("Monitor attached/detached readers. Press CTRL-C to abort.");
            Console.WriteLine("NOTE: Windows 10 restarts the PC/SC service after removing the last reader!");
            Console.WriteLine();

            var contextFactory = ContextFactory.Instance;
            using (var ctx = contextFactory.Establish(SCardScope.System)) {
                var readers = GetReaders(ctx);
                PrintReaders(readers);

                while (true) {
                    var scannerStates = new[] {
                        new SCardReaderState {
                            ReaderName = "\\\\?PnP?\\Notification",
                            CurrentStateValue = (IntPtr)(readers.Count << 16),
                            EventStateValue = (IntPtr)SCRState.Unknown,
                        }
                    };

                    var rc = ctx.GetStatusChange(ctx.Infinite, scannerStates);
                    if (rc != SCardError.Success) {
                        throw new PCSCException(rc);
                    }

                    var readersAfterStatusChange = GetReaders(ctx);
                    PrintRemovedReaders(readers, readersAfterStatusChange);
                    PrintAddedReaders(readers, readersAfterStatusChange);
                    readers = readersAfterStatusChange;
                }
            }
        }

        private static void PrintAddedReaders(ICollection<string> old, IEnumerable<string> @new) {
            foreach (var added in @new.Where(reader => !old.Contains(reader))) {
                Console.WriteLine($"New reader attached: {added}");
            }
        }

        private static void PrintRemovedReaders(IEnumerable<string> old, ICollection<string> @new) {
            foreach (var removed in old.Where(reader => !@new.Contains(reader))) {
                Console.WriteLine($"Reader detached: {removed}");
            }
        }

        private static HashSet<string> GetReaders(ISCardContext ctx) {
            return new HashSet<string>(ctx.GetReaders() ?? Enumerable.Empty<string>());
        }

        private static void PrintReaders(IEnumerable<string> readers) {
            Console.WriteLine("Current connected readers:");
            foreach (var name in readers) {
                Console.WriteLine(name);
            }
        }
    }
}
