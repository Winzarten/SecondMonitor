namespace SecondMonitor.ManagePlugins
{
    using System;
    using System.Windows;
    using Contracts.NInject;
    using Ninject.Parameters;
    using NLog;
    using PluginsConfiguration.Application.Controllers;
    using PluginsConfiguration.Application.View;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
                PluginsSettingsWindow pluginsSettingsWindow = new PluginsSettingsWindow();
                IPluginConfigurationController pluginConfigurationController = new KernelWrapper().Get<IPluginConfigurationController>(new ConstructorArgument("mainWindow", pluginsSettingsWindow));
                pluginsSettingsWindow.Closed += async (sender, args) =>
                {
                    await pluginConfigurationController.StopControllerAsync();
                    Current.Shutdown();
                };
                await pluginConfigurationController.StartControllerAsync();
                pluginsSettingsWindow.Show();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error Occured");
                Environment.Exit(1);
            }
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogManager.GetCurrentClassLogger().Error("Application experienced an unhandled excpetion");
            LogManager.GetCurrentClassLogger().Error(e.ExceptionObject);
        }
    }
}
