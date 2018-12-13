namespace SecondMonitor.PluginManager.Core
{
    public interface ISecondMonitorPlugin
    {
        PluginsManager PluginManager
        {
            get;
            set;
        }

        void RunPlugin();

        bool IsDaemon
        {
            get;
        }
    }
}
