using System;
using System.Windows.Input;

namespace RxEventsTestWpf
{
    internal class SimpleDelegateCommand : ICommand
    {
        private readonly Action<object> _action;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) {
            return true;
        }

        public void Execute(object parameter) {
            _action(parameter);
        }

        public void RaiseCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public SimpleDelegateCommand(Action<object> action) {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }
    }
}
