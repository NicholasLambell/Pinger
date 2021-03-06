using System;
using System.Windows.Input;

namespace Pinger {
    public class CommandHandler : ICommand {
        private Action<object> Action { get; }
        private bool AllowExecution { get; }

        public event EventHandler CanExecuteChanged;

        public CommandHandler(Action<object> action) : this(action, true) {}

        public CommandHandler(Action<object> action, bool canExecute) {
            Action = action;
            AllowExecution = canExecute;
        }

        public bool CanExecute(object parameter) {
            return AllowExecution;
        }

        public void Execute(object parameter) {
            Action(parameter);
        }
    }
}