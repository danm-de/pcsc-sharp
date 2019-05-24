using System;
using PCSC;
using PCSC.Monitoring;

namespace MonitorDeviceEvents {
    public class Program {
        public static void Main() {
            Console.WriteLine("Monitor attached/detached readers. Press SPACE key to exit.");
            Console.WriteLine();

            var factory = DeviceMonitorFactory.Instance;
            using (var monitor = factory.Create(SCardScope.System)) {
                monitor.Initialized += OnInitialized;
                monitor.StatusChanged += OnStatusChanged;
                monitor.MonitorException += OnMonitorException;

                monitor.Start();
                WaitUntilSpaceBarPressed();

                monitor.Initialized -= OnInitialized;
                monitor.StatusChanged -= OnStatusChanged;
                monitor.MonitorException -= OnMonitorException;
            }
        }

        private static void OnMonitorException(object sender, DeviceMonitorExceptionEventArgs args) {
            Console.WriteLine($"Exception: {args.Exception}");
        }

        private static void OnStatusChanged(object sender, DeviceChangeEventArgs e) {
            foreach (var removed in e.DetachedReaders) {
                Console.WriteLine($"Reader detached: {removed}");
            }

            foreach (var added in e.AttachedReaders) {
                Console.WriteLine($"New reader attached: {added}");
            }
        }

        private static void OnInitialized(object sender, DeviceChangeEventArgs e) {
            Console.WriteLine("Current connected readers:");
            foreach (var name in e.AllReaders) {
                Console.WriteLine(name);
            }
        }

        private static void WaitUntilSpaceBarPressed() {
            while (Console.ReadKey().Key != ConsoleKey.Spacebar) { }
        }
    }
}
