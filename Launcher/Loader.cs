namespace SecondMonitor.Launcher
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;

    using NLog;

    using SecondMonitor.PluginManager.Core;
    using SecondMonitor.PluginManager.GameConnector;

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
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                LoadUsingGameConnectorsFromDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConnectorsDir));
                Application.Run();

            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex, "Application experienced an error");
                MessageBox.Show(ex.ToString());
            }
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
                MessageBox.Show("No connectors loaded. Please place connectors .dll into " + connectorsDir, "No connectors", MessageBoxButtons.OK);
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
            PluginsManager pluginManager = new PluginsManager(connectors);
            pluginManager.InitializePlugins();
            pluginManager.Start();
        }
    }
}
