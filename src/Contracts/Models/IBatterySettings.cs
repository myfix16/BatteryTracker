namespace BatteryTracker.Contracts.Models
{
    /// <summary>
    /// Contains settings for battery notifications.
    /// </summary>
    public interface IBatterySettings
    {
        bool FullyChargedNotificationEnabled { get; set; }

        bool LowPowerNotificationEnabled { get; set; }

        int LowPowerNotificationThreshold { get; set; }

        bool HighPowerNotificationEnabled { get; set; }

        int HighPowerNotificationThreshold { get; set; }
    }
}
