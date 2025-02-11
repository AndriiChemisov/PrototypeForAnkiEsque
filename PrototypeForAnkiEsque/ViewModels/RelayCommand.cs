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
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute((T)parameter);

        // Executes the command with the given parameter
        public void Execute(object parameter)
        {
            // Check if the parameter is a string (to handle conversion to int)
            if (parameter is string stringParameter)
            {
                // Try to parse the string as an integer
                if (int.TryParse(stringParameter, out int parsedValue))
                {
                    _execute((T)(object)parsedValue); // Execute with the parsed integer value
                }
                else
                {
                    // Handle the case where conversion fails
                    Console.WriteLine($"Failed to convert parameter '{stringParameter}' to int.");
                }
            }
            else
            {
                // Handle the case where parameter is not a string or is of the wrong type
                Console.WriteLine($"Expected a string parameter, but got {parameter?.GetType().Name}.");
            }
        }
    }
}
