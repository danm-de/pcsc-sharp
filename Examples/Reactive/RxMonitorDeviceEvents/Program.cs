using System;
using System.Reactive.Linq;
using PCSC;
using PCSC.Monitoring;
using PCSC.Reactive;
using PCSC.Reactive.Events;

namespace RxMonitorDeviceEvents {
    public class Program {
        public static void Main() {
            Console.WriteLine("Listen device attached/detached events. Press any key to stop.");

            var factory = DeviceMonitorFactory.Instance;

            var subscription = factory
                .CreateObservable(SCardScope.System)
                .Select(GetEventAsPrintableText)
                .Do(Console.WriteLine)
                .Subscribe(
                    onNext: _ => { },
                    onError: OnError);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            subscription.Dispose();
        }

        private static void OnError(Exception exception) {
            Console.WriteLine("ERROR: {0}", exception.Message);
        }

        private static string GetEventAsPrintableText(DeviceMonitorEvent ev) {
            return $"Event type {ev.GetType().Name}, (readers: {string.Join(", ", ev.Readers)})";
        }
    }
}
