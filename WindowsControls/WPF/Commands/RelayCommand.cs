namespace SecondMonitor.WindowsControls.WPF.Commands
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

        public event EventHandler CanExecuteChanged;
    }
}