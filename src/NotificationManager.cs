using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

namespace BatteryTracker
{
    internal static class NotificationManager
    {
        internal static void PushMessage(string message)
        {
            var builder = new AppNotificationBuilder()
                .AddText(message);

            var notificationManager = AppNotificationManager.Default;
            notificationManager.Show(builder.BuildNotification());
        }
    }
}
