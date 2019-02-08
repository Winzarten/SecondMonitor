namespace SecondMonitor.PluginsConfiguration.Controller
{
    public interface IPluginSettingsProvider
    {
        bool TryIsPluginEnabled(string pluginName, out bool isEnabled);
        void SetPluginEnabled(string pluginName, bool isPluginEnabled);
    }
}