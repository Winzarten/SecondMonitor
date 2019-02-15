namespace SecondMonitor.ViewModels.PluginsSettings
{
    using PluginsConfiguration.Common.DataModel;

    public interface IPluginConfigurationViewModel : IViewModel<PluginConfiguration>
    {
        string PluginName { get; set; }

        bool IsEnabled { get; set; }
    }
}