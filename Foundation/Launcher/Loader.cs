namespace SecondMonitor.Launcher
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Contracts.NInject;
    using DataModel.Snapshot;
    using NLog;

    using PluginManager.Core;
    using PluginManager.GameConnector;
    using PluginsConfiguration.Common.Controller;
    using Application = System.Windows.Application;
    using MessageBox = System.Windows.MessageBox;

    public static class Loader
    {

        private static readonly string ConnectorsDir = "Connectors";


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            try
            {
                //Application.EnableVisualStyles();
                //Application.SetCompatibleTextRenderingDefault(false);
                AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
                Application app = new Application();
                LoadUsingGameConnectorsFromDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConnectorsDir));
                app.Run();

            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex, "Application experienced an error");
            }
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogManager.GetCurrentClassLogger().Error("Application experienced an unhandled excpetion");
            LogManager.GetCurrentClassLogger().Error(e.ExceptionObject);
        }

        private static void LoadUsingGameConnectorsFromDirectory(string connectorsDir)
        {
            IEnumerable<string> files = Directory.EnumerateFiles(connectorsDir, "*.dll", SearchOption.AllDirectories);
            var connectors = new List<IGameConnector>();
            foreach (var file in files)
            {
                string assemblyPath = file;
                connectors.AddRange(GetConnectorsFromAssembly(assemblyPath));

            }

            if (connectors.Count == 0)
            {
                MessageBox.Show("No connectors loaded. Please place connectors .dll into " + connectorsDir, "No connectors", System.Windows.MessageBoxButton.OK);
                Environment.Exit(1);
            }

            ConnectAndLoadPlugins(connectors.ToArray<IGameConnector>());
        }

        public static ICollection<IGameConnector> GetConnectorsFromAssembly(string assemblyPath)
        {
            var connectorType = typeof(IGameConnector);
            Assembly assembly;
            try
            {
                assembly = Assembly.UnsafeLoadFrom(assemblyPath);

            }
            catch (Exception)
            {
                return new List<IGameConnector>();
            }

            IEnumerable<Type> types = assembly.GetTypes().Where(c => !c.IsInterface && connectorType.IsAssignableFrom(c));
            return types.Select(type => Activator.CreateInstance(type) as IGameConnector).ToList();
        }

        private static void ConnectAndLoadPlugins(IGameConnector[] connectors)
        {
            KernelWrapper kernelWrapper = new KernelWrapper();
            IPluginSettingsProvider pluginSettingsProvider = kernelWrapper.Get<IPluginSettingsProvider>();
            IEnumerable<ISimulatorDateSetVisitor> dataVisitors = kernelWrapper.GetAll<ISimulatorDateSetVisitor>();
            PluginsManager pluginManager = new PluginsManager(pluginSettingsProvider, connectors, dataVisitors);
            pluginManager.InitializePlugins();
            pluginManager.Start();
        }
    }
}
