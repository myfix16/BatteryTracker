using System;
using System.Reflection;
using System.Windows.Forms;

namespace BatteryTrackerFramework
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var trayIcon = new TrayIcon();
            Application.Run();
        }
    }
}
