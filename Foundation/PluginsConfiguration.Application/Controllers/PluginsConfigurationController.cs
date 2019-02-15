namespace SecondMonitor.PluginsConfiguration.Application.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Common.Controller;
    using Common.Repository;
    using Contracts.Commands;
    using PluginManager.Core;
    using SecondMonitor.ViewModels.Factory;
    using SecondMonitor.ViewModels.PluginsSettings;
    using ViewModels;

    public class PluginsConfigurationController : IPluginConfigurationController
    {
        private readonly PluginsManager _pluginsManager;
        private readonly IPluginSettingsProvider _pluginSettingsProvider;
        private readonly IPluginConfigurationRepository _pluginConfigurationRepository;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly Window _mainWindow;
        private IPluginsSettingsWindowViewModel _pluginsSettingsWindowViewModel;

        private List<ISecondMonitorPlugin> _allPlugins;

        public PluginsConfigurationController(PluginsManager pluginsManager, IPluginSettingsProvider pluginSettingsProvider, IPluginConfigurationRepository pluginConfigurationRepository , IViewModelFactory viewModelFactory, Window mainWindow )
        {
            _pluginsManager = pluginsManager;
            _pluginSettingsProvider = pluginSettingsProvider;
            _pluginConfigurationRepository = pluginConfigurationRepository;
            _viewModelFactory = viewModelFactory;
            _mainWindow = mainWindow;
        }

        public async Task StartControllerAsync()
        {
            await Task.Run(() =>
            {
                InitializePluginsList();
                InitializeViewModels();
            });
            _mainWindow.DataContext = _pluginsSettingsWindowViewModel;
        }

        private void InitializeViewModels()
        {
            _pluginsSettingsWindowViewModel = _viewModelFactory.Create<IPluginsSettingsWindowViewModel>();
            IPluginsConfigurationViewModel pluginsConfigurationViewModel = _viewModelFactory.Create<IPluginsConfigurationViewModel>();
            pluginsConfigurationViewModel.FromModel(_pluginConfigurationRepository.LoadOrCreateDefault());
            _pluginsSettingsWindowViewModel.SaveCommand = new RelayCommand(Save);
            _pluginsSettingsWindowViewModel.PluginsConfigurationViewModel = pluginsConfigurationViewModel;
        }

        public Task StopControllerAsync()
        {
            return Task.CompletedTask;
        }

        private void Save()
        {
            _pluginConfigurationRepository.Save(_pluginsSettingsWindowViewModel.PluginsConfigurationViewModel.SaveToNewModel());
        }

        private void InitializePluginsList()
        {
            string pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            _allPlugins = _pluginsManager.GetPluginsFromPath(pluginsDirectory, true).ToList();
            foreach (ISecondMonitorPlugin secondMonitorPlugin in _allPlugins)
            {
                if (!_pluginSettingsProvider.TryIsPluginEnabled(secondMonitorPlugin.PluginName, out bool isEnabled))
                {
                    _pluginSettingsProvider.SetPluginEnabled(secondMonitorPlugin.PluginName, secondMonitorPlugin.IsEnabledByDefault);
                }
            }
        }
    }
}