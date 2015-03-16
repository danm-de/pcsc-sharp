using System;
using PCSC;

namespace MonitorReaderEvents
{
    public class Program
    {
        public static void Main() {
            Console.WriteLine("This program will monitor all SmartCard readers and display all status changes.");
            Console.WriteLine("Press a key to continue.");
            Console.ReadKey(); // Wait for user to press a key

            // Retrieve the names of all installed readers.
            string[] readerNames;
            using (var context = new SCardContext()) {
                context.Establish(SCardScope.System);
                readerNames = context.GetReaders();
                context.Release();
            }

            if (readerNames == null || readerNames.Length == 0) {
                Console.WriteLine("There are currently no readers installed.");
                return;
            }

            // Create a monitor object with its own PC/SC context. 
            // The context will be released after monitor.Dispose()
            var monitor = new SCardMonitor(new SCardContext(), SCardScope.System);
            // Point the callback function(s) to the anonymous & static defined methods below.
            monitor.CardInserted += (sender, args) => DisplayEvent("CardInserted", args);
            monitor.CardRemoved += (sender, args) => DisplayEvent("CardRemoved", args);
            monitor.Initialized += (sender, args) => DisplayEvent("Initialized", args);
            monitor.StatusChanged += StatusChanged;
            monitor.MonitorException += MonitorException;

            foreach (var reader in readerNames) {
                Console.WriteLine("Start monitoring for reader " + reader + ".");
            }

            monitor.Start(readerNames);

            // Let the program run until the user presses a key
            Console.ReadKey();

            // Stop monitoring
            monitor.Cancel();

            // Dispose monitor resources (SCardContext)
            monitor.Dispose();
        }

        private static void DisplayEvent(string eventName, CardStatusEventArgs unknown) {
            Console.WriteLine(">> {0} Event for reader: {1}", eventName, unknown.ReaderName);
            Console.WriteLine("ATR: {0}", BitConverter.ToString(unknown.Atr ?? new byte[0]));
            Console.WriteLine("State: {0}\n", unknown.State);
        }

        private static void StatusChanged(object sender, StatusChangeEventArgs args) {
            Console.WriteLine(">> StatusChanged Event for reader: {0}", args.ReaderName);
            Console.WriteLine("ATR: {0}", BitConverter.ToString(args.Atr ?? new byte[0]));
            Console.WriteLine("Last state: {0}\nNew state: {1}\n", args.LastState, args.NewState);
        }

        private static void MonitorException(object sender, PCSCException ex) {
            Console.WriteLine("Monitor exited due an error:");
            Console.WriteLine(SCardHelper.StringifyError(ex.SCardError));
        }
    }
}