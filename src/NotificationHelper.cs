using System;
using Microsoft.Toolkit.Uwp.Notifications;

namespace BatteryTracker
{
    internal static class NotificationHelper
    {
        internal static void PushLowPowerNotification(int percentage, int remainingSeconds)
        {
            new ToastContentBuilder()
                .AddText($"Low power, {percentage}% remaining")
                .AddText($"Estimated remaining battery time: {TimeSpan.FromSeconds(remainingSeconds)}")
                .Show();
        }

        internal static void PushChargedNotification()
        {
            new ToastContentBuilder()
                .AddText("The battery is fully charged⚡")
                .Show();
        }
    }
}
