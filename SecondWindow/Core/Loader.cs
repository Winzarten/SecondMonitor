using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SecondMonitor.Core.R3EConnector;
using SecondMonitor.Core.PluginManager;

namespace SecondMonitor
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
