using System;
using System.Windows.Forms;
using SecondMonitor.PluginManager.GameConnector;
using SecondMonitor.PluginManager.Core;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using SecondMonitor.Launcher.Dialog;

namespace SecondMonitor.Launcher
{
    static class Loader
    {
        //private static readonly string defaultGameConnector = "PCarsConnector.dll";// "R3EConnector.dll";
        //private static readonly string defaultGameConnector = "Connectors\\R3E\\R3EConnector.dll";
        private static readonly string connectorsDir = "Connectors";
        //private static readonly string defaultGameConnector = "Connectors\\Mocked\\MockedConnector.dll";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //string gameConnector = defaultGameConnector;
                //WaitingDialog dialog = new WaitingDialog();
                
                //while (String.IsNullOrEmpty(gameConnector))
                //{
                    //if (Process.GetProcessesByName("RRRE").Length > 0)
                      //  gameConnector = "R3EConnector.dll";

                    //if (Process.GetProcessesByName("pCARS64").Length > 0)
                      //  gameConnector = "PCarsConnector.dll";
                    //Thread.Sleep(100);
                    //if (dialog.DialogResult == DialogResult.Cancel)
                        //Environment.Exit(0);
                //}
                //dialog.Close();
                //dialog.Dispose();
                LoadUsingGameConnectorsFromDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,connectorsDir));
                Application.Run();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }                
        }

        private static void LoadUsingGameConnectorsFromDirectory(string connectorsDir)
        {
            IEnumerable<string> files = Directory.EnumerateFiles(connectorsDir, "*.dll", SearchOption.AllDirectories);
            List<IGameConnector> connectors = new List<IGameConnector>();
            foreach (String file in files)
            {
                string assemblyPath = file;
                connectors.AddRange(GetConnectorsFromAssembly(assemblyPath));

            }
            if (connectors.Count == 0)
            {
                MessageBox.Show("No connectors loaded. Please place connectores .dll into " + connectorsDir, "No connectos", MessageBoxButtons.OK);
                System.Environment.Exit(1);
            }
            ConnectAndLoadPlugins(connectors.ToArray<IGameConnector>());
        }

        public static ICollection<IGameConnector> GetConnectorsFromAssembly(string assemblyPath)
        {
            var connectorType = typeof(IGameConnector);
            List<IGameConnector> plugins = new List<IGameConnector>();
            Assembly assembly;
            try
            {
                assembly = Assembly.UnsafeLoadFrom(assemblyPath);

            }
            catch (Exception)
            {
                return new List<IGameConnector>();
            }
            IEnumerable<Type> types = assembly.GetTypes().Where(c => !(c.IsInterface) && connectorType.IsAssignableFrom(c));
            foreach (Type type in types)
            {
                IGameConnector connectors = Activator.CreateInstance(type) as IGameConnector;
                plugins.Add(connectors);
            }
            return plugins;

        }
       
        private static void ConnectAndLoadPlugins(IGameConnector[] connectors)
        {
            PluginsManager pluginManager = new PluginsManager(connectors);
            pluginManager.InitializePlugins();
            pluginManager.Start();
        }
    }
}
