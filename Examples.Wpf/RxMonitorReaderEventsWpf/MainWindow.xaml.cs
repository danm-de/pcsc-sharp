using System;
using System.Windows;
using PCSC;
using PCSC.Monitoring;

namespace RxMonitorReaderEventsWpf
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow() {
            InitializeComponent();

            _viewModel = new MainWindowViewModel(ContextFactory.Instance, MonitorFactory.Instance);
            DataContext = _viewModel;
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            _viewModel.Dispose();
        }
    }
}
