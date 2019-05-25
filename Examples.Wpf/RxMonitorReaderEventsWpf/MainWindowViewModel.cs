using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using PCSC;
using PCSC.Monitoring;
using PCSC.Reactive;
using PCSC.Reactive.Events;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace RxMonitorReaderEventsWpf {
    public class MainWindowViewModel : IDisposable {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private IDisposable _subscription;

        public ReactiveCollection<MonitorEvent> EventHistory { get; }
        public ReactiveCollection<string> Readers { get; }
        public ReactiveCommand RefreshReaderListCommand { get; }
        public ReactiveCommand ClearEventListCommand { get; }
        
        public MainWindowViewModel(IContextFactory contextFactory, IMonitorFactory monitorFactory) {
            Readers = new ReactiveCollection<string>().AddTo(_disposables);
            EventHistory = new ReactiveCollection<MonitorEvent>().AddTo(_disposables);
            RefreshReaderListCommand = new ReactiveCommand().AddTo(_disposables);
            ClearEventListCommand = new ReactiveCommand().AddTo(_disposables);

            RefreshReaderListCommand
                .Select(_ => GetReaderNames(contextFactory))
                .Do(UpdateReaderList)
                .Do(readerNames => SubscribeToReaderEvents(monitorFactory, readerNames))
                .Subscribe()
                .AddTo(_disposables);

            ClearEventListCommand
                .Do(_ => EventHistory.ClearOnScheduler())
                .Subscribe()
                .AddTo(_disposables);
        }


        private static string[] GetReaderNames(IContextFactory contextFactory) {
            using var ctx = contextFactory.Establish(SCardScope.System);
            return ctx.GetReaders();
        }

        private void UpdateReaderList(IEnumerable<string> readerNames) {
            Readers.ClearOnScheduler();
            EventHistory.ClearOnScheduler();
            Readers.AddRangeOnScheduler(readerNames);
        }

        private void SubscribeToReaderEvents(IMonitorFactory monitorFactory, IReadOnlyCollection<string> readerNames) {
            _subscription?.Dispose();

            if (readerNames.Count <= 0) {
                return;
            }

            _subscription = monitorFactory
                .CreateObservable(SCardScope.System, readerNames)
                .Do(ev => EventHistory.AddOnScheduler(ev)) // Always add elements using the UI scheduler!
                .Subscribe(
                    onNext: _ => { },
                    onError: OnError);
        }

        private void OnError(Exception exception) {
            Debug.Print(exception.Message);
            Readers.ClearOnScheduler();
            EventHistory.ClearOnScheduler();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }

            _subscription?.Dispose();
            _disposables.Dispose();
        }
    }
}
