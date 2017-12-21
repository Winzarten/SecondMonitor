using System;
using System.Windows.Input;

namespace SecondMonitor.Timing.DataHandler.Commands
{
    public class TimingModeChangedCommand : ICommand    
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        private readonly Action _executeDelegate;
        private readonly Func<bool> _canExecuteDelegate;

        public TimingModeChangedCommand(Action execute)
        {
            _executeDelegate = execute;
            _canExecuteDelegate = () => true;
        }

        public TimingModeChangedCommand(Action execute, Func<bool> canExecute)
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
