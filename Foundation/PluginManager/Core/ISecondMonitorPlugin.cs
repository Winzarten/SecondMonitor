namespace SecondMonitor.PluginManager.Core
{
    public interface ISecondMonitorPlugin
    {
        PluginsManager PluginManager
        {
            get;
            set;
        }

        string PluginName { get; }
        bool IsEnabledByDefault { get; }

        void RunPlugin();

        bool IsDaemon
        {
            get;
        }
    }
}
