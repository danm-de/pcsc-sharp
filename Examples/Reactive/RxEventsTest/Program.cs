using System;
using System.Linq;
using System.Reactive.Linq;
using PCSC;
using PCSC.Reactive;
using PCSC.Reactive.Events;

namespace RxEventsTest
{
    class Program
    {
        static void Main(string[] args) {
            var readers = GetReaders();

            if (!readers.Any()) {
                Console.WriteLine("You need at least one connected smart card reader.");
                Console.ReadKey();
                return;
            }
            
            Console.WriteLine("Listen to all reader events. Press any key to stop.");

            var monitorFactory = MonitorFactory.Instance;
            var subscription = monitorFactory
                .CreateObservable(SCardScope.System, readers)
                .Subscribe(OnNext, OnError);

            Console.ReadKey();
            subscription.Dispose();
        }

        private static void OnError(Exception exception) {
            Console.WriteLine("ERROR: {0}", exception.Message);
        }

        private static void OnNext(MonitorEvent ev) {
            Console.WriteLine($"Event type {ev.GetType()}, reader: {ev.ReaderName}");
        }

        private static string[] GetReaders() {
            var contextFactory = ContextFactory.Instance;
            using (var ctx = contextFactory.Establish(SCardScope.System)) {
                return ctx.GetReaders();
            }
        }
    }
}
