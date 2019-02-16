namespace SecondMonitor.PluginsConfiguration.Common.Controller
{
    using DataModel;

    public interface IPluginSettingsProvider
    {
        bool TryIsPluginEnabled(string pluginName, out bool isEnabled);
        void SetPluginEnabled(string pluginName, bool isPluginEnabled);
        RemoteConfiguration RemoteConfiguration { get; }
    }
}