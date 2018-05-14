using System;
using System.Windows.Input;

namespace MasterThesisApplication.Utility
{
    public class CustomCommand : ICommand
    {
        private readonly Action<object> _executeAction;
        private readonly Predicate<object> _canExecutePredicate;

        public CustomCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _executeAction = execute;
            _canExecutePredicate = canExecute;
        }
        public bool CanExecute(object parameter)
        {
            return _canExecutePredicate?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _executeAction(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
    }
}
