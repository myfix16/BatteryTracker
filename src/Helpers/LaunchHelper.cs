using System.Diagnostics;

namespace BatteryTracker.Helpers
{
    public static class LaunchHelper
    {
        public const string ColorsSettingsUri = "ms-settings:colors";
        public const string GitHubRepoUri = "https://github.com/myfix16/BatteryTracker";
        public const string GitHubNewIssueUri = "https://github.com/myfix16/BatteryTracker/issues/new/choose";
        public const string PrivacyStatementUri = "https://github.com/myfix16/BatteryTracker/blob/main/Privacy.md";
        public const string EmailFeedbackUri = "mailto:myfix16@outlook.com?subject=Battery%20Tracker%20Feedback";
        public const string StoreRatingUri = "ms-windows-store:review/?ProductId=9P1FBSLRNM43";
        public const string TranslationUri = "https://hosted.weblate.org/projects/battery-tracker/app/";

        public static Process? StartProcess(string process)
        {
            return Process.Start(new ProcessStartInfo(process) { UseShellExecute = true });
        }

        public static async Task<bool> LaunchUriAsync(string uri)
        {
            return await Windows.System.Launcher.LaunchUriAsync(new Uri(uri));
        }
    }
}
