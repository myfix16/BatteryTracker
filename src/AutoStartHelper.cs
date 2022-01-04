using System.Windows.Forms;
using Microsoft.Win32;

namespace BatteryTracker
{
    internal static class AutoStartHelper
    {
        //Startup registry key and value
        const string StartupKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        internal static bool IsRunAtStartup(string startupValue)
        {
            using RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, false);
            return key?.GetValue(startupValue) != null;
        }

        internal static void EnableStartup(string startupValue)
        {
            //Set the application to run at startup
            using RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
            key?.SetValue(startupValue, Application.ExecutablePath);
        }

        internal static void DisableStartup(string startupValue)
        {
            //Set the application to run at startup
            using RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
            key?.DeleteValue(startupValue);
        }
    }
}
