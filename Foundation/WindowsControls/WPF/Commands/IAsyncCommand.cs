namespace SecondMonitor.WindowsControls.WPF.Commands
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}