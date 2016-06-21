using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SecondWindow.Core.R3EConnector;

namespace SecondWindow
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
                IR3EConnector connector = new R3EConnector();
                connector.AsynConnect();
                /*Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());*/
            }catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

                
        }
    }
}
