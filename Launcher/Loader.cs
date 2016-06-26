using System;
using System.Windows.Forms;
using SecondMonitor.Core.R3EConnector;

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
                
                IR3EConnector connector = new R3EConnector();
                PluginManager pluginManager = new PluginManager(connector);
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
