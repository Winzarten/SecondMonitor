namespace SecondMonitor.Contracts.Commands
{
    using System;
    using System.Windows.Input;

    public class RelayCommand : ICommand
    {
        private readonly Action _relayAction;

        public RelayCommand(Action action)
        {
            _relayAction = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _relayAction();
        }

// Interface implementation
#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning disable CS0067
    }
}