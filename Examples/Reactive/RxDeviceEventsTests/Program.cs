using System;
using PCSC;
using PCSC.Monitoring;
using PCSC.Reactive;
using PCSC.Reactive.Events;

namespace RxDeviceEventsTests
{
    public class Program
    {
        public static void Main() {
            Console.WriteLine("Listen device attached/detached events. Press any key to stop.");

            var factory = DeviceMonitorFactory.Instance;

            var subscription = factory
                .CreateObservable(SCardScope.System)
                .Subscribe(OnNext, OnError);

            Console.ReadKey();
            subscription.Dispose();
        }

        private static void OnError(Exception exception) {
            Console.WriteLine("ERROR: {0}", exception.Message);
        }

        private static void OnNext(DeviceMonitorEvent ev) {
            Console.WriteLine($"Event type {ev.GetType()}, (readers: {string.Join(", ",ev.Readers)})");
        }
    }
}
