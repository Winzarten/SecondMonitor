namespace SecondMonitor.Timing.Presentation.ViewModel.Commands
{
    using System;
    using System.Windows.Input;

    public class NoArgumentCommand : ICommand
    {        
        private readonly Action _executeDelegate;

        private Func<bool> _canExecuteDelegate;

        public event EventHandler CanExecuteChanged;

        public NoArgumentCommand(Action execute)
        {
            _executeDelegate = execute;
            _canExecuteDelegate = () => true;
        }

        public NoArgumentCommand(Action execute, Func<bool> canExecute)
        {
            _executeDelegate = execute;
            _canExecuteDelegate = canExecute;
        }

        public Func<bool> CanExecuteDelegate
        {
            set
            {
                _canExecuteDelegate = value;
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
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
