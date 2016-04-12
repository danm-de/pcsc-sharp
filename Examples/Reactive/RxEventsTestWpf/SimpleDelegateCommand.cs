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

        public SimpleDelegateCommand(Action<object> action) {
            if (action == null) {
                throw new ArgumentNullException(nameof(action));
            }
            _action = action;
        }
    }
}