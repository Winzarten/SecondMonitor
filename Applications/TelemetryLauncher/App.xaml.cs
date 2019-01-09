namespace SecondMonitor.TelemetryLauncher
{
    using System;
    using System.Windows;
    using NLog;
    using Telemetry.TelemetryApplication.Controllers;
    using TelemetryPresentation.MainWindow;

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
                MainWindow mainWindow = new MainWindow();
                TelemetryApplicationController telemetryApplicationController = new TelemetryApplicationController(mainWindow);
                telemetryApplicationController.StartController();
                mainWindow.Closed += (sender, args) =>
                {
                    telemetryApplicationController.StopController();
                    Current.Shutdown();
                };
                await telemetryApplicationController.OpenLastSessionFromRepository();
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
