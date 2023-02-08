using System;
using System.Windows.Input;

namespace AlarmViewProyect
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;



        public RelayCommand(Action<object> execute) : this(execute, null)
        {
            _execute = execute;
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }
            else
            {
                //if (parameter == null)
                //   throw new ArgumentNullException(null, "parameter is null");
                return _canExecute(parameter);
            }
        }
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
