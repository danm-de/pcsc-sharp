using System;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using PCSC;
using PCSC.Monitoring;
using PCSC.Reactive;
using PCSC.Reactive.Events;

namespace RxMonitorReaderEvents {
    public class Program {
        public static void Main() {
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
                .Select(GetEventText)
                .Do(Console.WriteLine)
                .Subscribe(
                    onNext: _ => { },
                    onError: OnError);

            Console.ReadKey();
            subscription.Dispose();
        }

        private static void OnError(Exception exception) {
            Console.Error.WriteLine("ERROR: {0}", exception.Message);
        }

        private static string GetEventText(MonitorEvent ev) {
            var sb = new StringBuilder();
            sb.Append($"{ev.GetType().Name} - reader: {ev.ReaderName}");
            switch (ev) {
                case CardStatusChanged changed:
                    sb.AppendLine($", previous state: {changed.PreviousState}, new state: {changed.NewState}");
                    break;
                case CardRemoved removed:
                    sb.AppendLine($", state: {removed.State}");
                    break;
                case CardInserted inserted:
                    sb.AppendLine($", state: {inserted.State}, ATR: {BitConverter.ToString(inserted.Atr)}");
                    break;
                case MonitorInitialized initialized:
                    sb.AppendLine($", state: {initialized.State}, ATR: {BitConverter.ToString(initialized.Atr)}");
                    break;
                case MonitorCardInfoEvent infoEvent:
                    sb.AppendLine($", state: {infoEvent.State}, ATR: {BitConverter.ToString(infoEvent.Atr)}");
                    break;
            }

            return sb.ToString();
        }

        private static string[] GetReaders() {
            var contextFactory = ContextFactory.Instance;
            using (var ctx = contextFactory.Establish(SCardScope.System)) {
                return ctx.GetReaders();
            }
        }
    }
}
