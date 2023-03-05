using System.Diagnostics;

namespace BatteryTracker.Helpers
{
    public static class LaunchHelper
    {
        public const string ColorsSettingsUri = "ms-settings:colors";

        public static void StartProcess(string process)
        {
            Process.Start(new ProcessStartInfo(process) { UseShellExecute = true });
        }

        public static async Task<bool> LaunchUriAsync(string uri)
        {
            return await Windows.System.Launcher.LaunchUriAsync(new Uri(uri));
        }
    }
}
