namespace SecondMonitor.ViewModels.PluginsSettings
{
    using System.Collections.Generic;
    using System.Linq;
    using Factory;
    using PluginsConfiguration.Common.DataModel;

    public class PluginsConfigurationViewModel : AbstractViewModel<PluginsConfiguration>, IPluginsConfigurationViewModel
    {
        private readonly IViewModelFactory _viewModelFactory;
        private IRemoteConfigurationViewModel _remoteConfigurationViewModel;
        private IReadOnlyCollection<IPluginConfigurationViewModel> _pluginConfigurations;

        public PluginsConfigurationViewModel(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public IRemoteConfigurationViewModel RemoteConfigurationViewModel
        {
            get => _remoteConfigurationViewModel;
            private set => SetProperty(ref _remoteConfigurationViewModel, value);
        }

        public IReadOnlyCollection<IPluginConfigurationViewModel> PluginConfigurations
        {
            get => _pluginConfigurations;
            private set => SetProperty(ref _pluginConfigurations, value);
        }

        protected override void ApplyModel(PluginsConfiguration model)
        {
            IRemoteConfigurationViewModel newViewModel = _viewModelFactory.Create<IRemoteConfigurationViewModel>();
            newViewModel.FromModel(model.RemoteConfiguration);
            RemoteConfigurationViewModel = newViewModel;

            List<IPluginConfigurationViewModel> newPluginsViewModel = new List<IPluginConfigurationViewModel>(model.PluginsConfigurations.Count);
            foreach (PluginConfiguration modelPluginsConfiguration in model.PluginsConfigurations)
            {
                IPluginConfigurationViewModel newPluginConfigurationViewModel = _viewModelFactory.Create<IPluginConfigurationViewModel>();
                newPluginConfigurationViewModel.FromModel(modelPluginsConfiguration);
                newPluginsViewModel.Add(newPluginConfigurationViewModel);
            }

            PluginConfigurations = newPluginsViewModel;

        }

        public override PluginsConfiguration SaveToNewModel()
        {
            return new PluginsConfiguration()
            {
                RemoteConfiguration = RemoteConfigurationViewModel.SaveToNewModel(),
                PluginsConfigurations = PluginConfigurations.Select(x => x.SaveToNewModel()).ToList(),

            };
        }
    }
}