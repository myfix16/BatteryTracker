using System;
using System.Threading;
using System.Windows.Forms;

namespace BatteryTracker
{
    static class Program
    {
        public static readonly string AppName = Application.ProductName;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using var mutex = new Mutex(true, AppName + "Singleton", out bool notAlreadyRunning);
            if (notAlreadyRunning)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                var trayIcon = new TrayIcon();
                Application.Run();
            }
            else MessageBox.Show($"{AppName} is already running.");
        }
    }
}
