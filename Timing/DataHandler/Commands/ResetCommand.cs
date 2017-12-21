using System;
using System.Windows.Input;

namespace SecondMonitor.Timing.DataHandler.Commands
{
    public class NoArgumentCommand : ICommand
    {        
        private readonly Action _executeDelegate;

        private readonly Func<bool> _canExecuteDelegate;

        public event EventHandler CanExecuteChanged;

        public NoArgumentCommand(Action execute)
        {
            _executeDelegate = execute;
            _canExecuteDelegate = () => { return true; };
        }

        public NoArgumentCommand(Action execute, Func<bool> canExecute)
        {
            _executeDelegate = execute;
            _canExecuteDelegate = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteDelegate();
        }

        public void Execute(object parameter)
        {
            _executeDelegate();
        }
    }
}
