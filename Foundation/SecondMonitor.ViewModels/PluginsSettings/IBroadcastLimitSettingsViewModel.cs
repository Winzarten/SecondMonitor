using SecondMonitor.PluginsConfiguration.Common.DataModel;

namespace SecondMonitor.ViewModels.PluginsSettings
{
    public interface IBroadcastLimitSettingsViewModel : IViewModel<BroadcastLimitSettings>
    {
        bool IsEnabled { get; set; }

        int MinimumPackageInterval { get; set; }

        int PlayerTimingPackageInterval { get; set; }

        int OtherDriversTimingPackageInterval { get; set; }
    }
}