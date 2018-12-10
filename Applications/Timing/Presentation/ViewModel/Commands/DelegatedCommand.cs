namespace SecondMonitor.Timing.Presentation.ViewModel.Commands
{
    using System;
    using System.Windows.Input;

    public class DelegatedCommand : ICommand    
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        private readonly Action _executeDelegate;
        private readonly Func<bool> _canExecuteDelegate;

        public DelegatedCommand(Action execute)
        {
            _executeDelegate = execute;
            _canExecuteDelegate = () => true;
        }

        public DelegatedCommand(Action execute, Func<bool> canExecute)
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
