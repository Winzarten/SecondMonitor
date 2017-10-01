using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SecondMonitor.Timing.DataHandler.Commands
{
    public class TimingModeChangedCommand : ICommand    
    {
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
        private Action executeDelegate;
        private Func<bool> canExecuteDelegate;

        public TimingModeChangedCommand(Action execute)
        {
            executeDelegate = execute;
            canExecuteDelegate = () => { return true; };
        }
        public TimingModeChangedCommand(Action execute, Func<bool> canExecute)
        {
            executeDelegate = execute;
            canExecuteDelegate = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecuteDelegate();
        }

        public void Execute(object parameter)
        {
            executeDelegate();
        }
    }
}
