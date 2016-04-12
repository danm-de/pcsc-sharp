using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Input;
using PCSC;
using PCSC.Reactive;
using PCSC.Reactive.Events;

namespace RxEventsTestWpf
{
    public class MainWindowViewModel : IDisposable
    {
        private readonly IContextFactory _contextFactory = ContextFactory.Instance;
        private readonly IMonitorFactory _monitorFactory = MonitorFactory.Instance;

        public ObservableCollection<MonitorEvent> EventHistory { get; }
        public ObservableCollection<string> Readers { get; } 
        public ICommand RefreshReaderList { get; }

        private readonly IScheduler _uiScheduler;
        private IDisposable _subscription;

        public MainWindowViewModel() {
            Readers = new ObservableCollection<string>();
            EventHistory = new ObservableCollection<MonitorEvent>();
            RefreshReaderList = new SimpleDelegateCommand(UpdateReaderList);
            _uiScheduler = DispatcherScheduler.Current;
        }

        private void UpdateReaderList(object parameter) {
            Readers.Clear();
            EventHistory.Clear();
            using (var ctx = _contextFactory.Establish(SCardScope.System)) {
                foreach (var reader in ctx.GetReaders()) {
                    Readers.Add(reader);
                }
            }
            _subscription?.Dispose();
            if (Readers.Count > 0) {
                _subscription = _monitorFactory
                    .CreateObservable(SCardScope.System, Readers)
                    .ObserveOn(_uiScheduler) // Important! Monitor does not run on UI thread.
                    .Subscribe(EventHistory.Add);
            }
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
        }
    }
}