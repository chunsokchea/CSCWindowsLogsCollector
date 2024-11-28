using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace CSCWindowsLogsCollector
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new frmInsertLogs());
            bool createdNew;
            using (Mutex mutex = new Mutex(true, "CSCWindowsLogs", out createdNew))
            {
                if (!createdNew)
                {
                    MessageBox.Show("Another instance of the application is already running.", "Instance Already Running",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmInsertLogs());
            }
        }
    }
}
