using System;
using System.Windows.Input;

namespace PrototypeForAnkiEsque.ViewModels
{
    // RelayCommand without parameter
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        // Constructor to execute an action without a parameter
        public RelayCommand(Action execute) : this(execute, null) { }

        // Constructor to execute an action without a parameter and with canExecute logic
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Event for CanExecute state changes
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        // Returns if the command can be executed
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        // Executes the action
        public void Execute(object parameter)
        {
            _execute();
        }

        // Call this method when you want to notify that CanExecute has changed
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    // RelayCommand with parameter
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        // Constructor to execute an action with a parameter
        public RelayCommand(Action<T> execute) : this(execute, null) { }

        // Constructor to execute an action with a parameter and with canExecute logic
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Event for CanExecute state changes
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        // Returns if the command can be executed with the given parameter
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        // Executes the command with the given parameter
        public void Execute(object parameter)
        {
            if (parameter is T param)
            {
                _execute(param);
            }
            else
            {
                // Log error or handle invalid parameter
                Console.WriteLine($"Expected parameter of type {typeof(T)}, but got {parameter?.GetType().Name}.");
            }
        }

        // Call this method when you want to notify that CanExecute has changed
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
