namespace SecondMonitor.PluginsConfiguration.Application.ViewModels
{
    using System.Windows.Input;
    using SecondMonitor.ViewModels;
    using SecondMonitor.ViewModels.PluginsSettings;

    public interface IPluginsSettingsWindowViewModel : IViewModel
    {
        IPluginsConfigurationViewModel PluginsConfigurationViewModel { get; set; }
        ICommand SaveCommand { get; set; }
    }
}