using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SecondMonitor.Timing.DataHandler.Commands
{
    public class ResetCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action executeDelegate;
        private Func<bool> canExecuteDelegate;

        public ResetCommand(Action execute)
        {
            executeDelegate = execute;
            canExecuteDelegate = () => { return true; };
        }
        public ResetCommand(Action execute, Func<bool> canExecute)
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
