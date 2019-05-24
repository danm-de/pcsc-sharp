using System;
using System.Windows;

namespace RxEventsTestWpf
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel = new MainWindowViewModel();

        public MainWindow() {
            InitializeComponent();
            DataContext = _viewModel;
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            _viewModel.Dispose();
        }
    }
}
