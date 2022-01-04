using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Popups;

namespace BatteryTracker
{
    internal static class AutoStartHelper
    {
        internal static async Task<bool> IsRunAtStartup()
        {
            StartupTaskState startupTask = (await StartupTask.GetAsync("BatteryTrackerStartup")).State;
            return startupTask == StartupTaskState.Enabled || startupTask == StartupTaskState.EnabledByPolicy;
        }

        internal static async Task DisableStartup()
        {
            StartupTask startupTask = await StartupTask.GetAsync("BatteryTrackerStartup");
            switch (startupTask.State)
            {
                case StartupTaskState.Enabled:
                    startupTask.Disable();
                    break;
                case StartupTaskState.EnabledByPolicy:
                    var dialog = new MessageDialog("Startup enabled by group policy, or not supported on this device");
                    await dialog.ShowAsync();
                    break;
            }
        }

        internal static async Task EnableStartup()
        {
            StartupTask startupTask = await StartupTask.GetAsync("BatteryTrackerStartup");
            switch (startupTask.State)
            {
                // Task is disabled but can be enabled.
                case StartupTaskState.Disabled:
                    await startupTask.RequestEnableAsync();
                    break;
                case StartupTaskState.DisabledByPolicy:
                    var dialog = new MessageDialog("Startup disabled by group policy, or not supported on this device");
                    await dialog.ShowAsync();
                    break;
            }
        }

        // //Startup registry key and value
        // const string StartupKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        //
        // internal static bool IsRunAtStartup(string startupValue)
        // {
        //     using RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, false);
        //     return key?.GetValue(startupValue) != null;
        // }
        //
        // internal static void EnableStartup(string startupValue)
        // {
        //     //Set the application to run at startup
        //     using RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
        //     key?.SetValue(startupValue, Application.ExecutablePath);
        // }
        //
        // internal static void DisableStartup(string startupValue)
        // {
        //     //Set the application to run at startup
        //     using RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
        //     key?.DeleteValue(startupValue);
        // }
    }
}
