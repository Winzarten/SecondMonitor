namespace SecondMonitor.PluginsConfiguration.Application.ViewModels
{
    using System.Windows.Input;
    using SecondMonitor.ViewModels;
    using SecondMonitor.ViewModels.PluginsSettings;

    public class PluginsSettingsWindowViewModel : AbstractViewModel, IPluginsSettingsWindowViewModel
    {
        private IPluginsConfigurationViewModel _pluginsConfigurationViewModel;
        private ICommand _saveCommand;
        private ICommand _closeCommand;

        public IPluginsConfigurationViewModel PluginsConfigurationViewModel
        {
            get => _pluginsConfigurationViewModel;
            set => SetProperty(ref _pluginsConfigurationViewModel, value);
        }

        public ICommand SaveCommand
        {
            get => _saveCommand;
            set => SetProperty(ref _saveCommand, value);
        }

        public ICommand CloseCommand
        {
            get => _saveCommand;
            set => SetProperty(ref _closeCommand, value);
        }
    }
}