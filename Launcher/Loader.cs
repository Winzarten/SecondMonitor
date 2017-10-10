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
        private static readonly string defaultGameConnector = "R3EConnector.dll";
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
                string gameConnector = defaultGameConnector;
                //WaitingDialog dialog = new WaitingDialog();
                
                //while (String.IsNullOrEmpty(gameConnector))
                //{
                    if (Process.GetProcessesByName("RRRE").Length > 0)
                        gameConnector = "R3EConnector.dll";

                    if (Process.GetProcessesByName("pCARS64").Length > 0)
                        gameConnector = "PCarsConnector.dll";
                    Thread.Sleep(100);
                    //if (dialog.DialogResult == DialogResult.Cancel)
                        //Environment.Exit(0);
                //}
                //dialog.Close();
                //dialog.Dispose();
                LoadUsingGameConnectorAssembply(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, gameConnector));
                Application.Run();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }                
        }

        private static void LoadUsingGameConnectorAssembply(string assemblyPath)
        {
            Assembly assembly = Assembly.LoadFile(assemblyPath);
            LoadUsingGameConnectorAssembply(assembly);
        }
        private static void LoadUsingGameConnectorAssembply(Assembly assembly)
        {
            var connectorPluginType = typeof(IGameConnector);                          
            IEnumerable<Type> types = assembly.GetTypes().Where(c => !(c.IsInterface) && connectorPluginType.IsAssignableFrom(c));
            foreach (Type type in types)
            {
                IGameConnector gameConnector = Activator.CreateInstance(type) as IGameConnector;
                ConnectAndLoadPlugins(gameConnector);
                return;
            }
            
        }
        private static void ConnectAndLoadPlugins(IGameConnector connector)
        {
            
            PluginsManager pluginManager = new PluginsManager(connector);
            pluginManager.InitializePlugins();                             
            connector.AsynConnect();
        }
    }
}
