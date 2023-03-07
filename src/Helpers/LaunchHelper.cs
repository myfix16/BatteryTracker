using System.Diagnostics;

namespace BatteryTracker.Helpers
{
    public static class LaunchHelper
    {
        public const string ColorsSettingsUri = "ms-settings:colors";
        public const string GitHubRepoUri = "https://github.com/myfix16/BatteryTracker";
        public const string GitHubNewIssueUri = "https://github.com/myfix16/BatteryTracker/issues/new/choose";
        public const string PrivacyStatementUri = "https://github.com/myfix16/BatteryTracker/blob/main/Privacy.md";

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
