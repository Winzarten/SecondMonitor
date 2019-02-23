namespace SecondMonitor.ViewModels.PluginsSettings
{
    using PluginsConfiguration.Common.DataModel;

    public class PluginConfigurationViewModel : AbstractViewModel<PluginConfiguration>, IPluginConfigurationViewModel
    {
        private string _pluginName;
        private bool _isEnabled;

        public string PluginName
        {
            get => _pluginName;
            set => SetProperty(ref _pluginName, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }


        protected override void ApplyModel(PluginConfiguration model)
        {
            PluginName = model.PluginName;
            IsEnabled = model.IsEnabled;
        }

        public override PluginConfiguration SaveToNewModel()
        {
            return new PluginConfiguration()
            {
                PluginName = PluginName,
                IsEnabled = IsEnabled
            };
        }
    }
}