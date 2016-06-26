using System;
using System.Windows.Forms;
using SecondMonitor.Core.R3EConnector;
using SecondMonitor.PluginManager.GameConnector;
using SecondMonitor.PluginManager.Core;

namespace SecondMonitor.Launcher
{
    static class Program
    {
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
                
                IGameConnector connector = new R3EConnector();
                PluginsManager pluginManager = new PluginsManager(connector);
                pluginManager.InitializePlugins();                             
                connector.AsynConnect();

                Application.Run();

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

                
        }
    }
}
