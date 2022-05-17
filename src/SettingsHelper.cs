using Windows.Storage;

namespace BatteryTracker
{
    internal static class SettingsHelper
    {
        public const string ShowNotificationSetting = "showNotificationSetting";

        public static ApplicationDataContainer GetSettings() => ApplicationData.Current.LocalSettings;
    }
}
