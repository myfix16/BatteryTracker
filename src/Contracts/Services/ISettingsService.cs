using System.Collections.Generic;

namespace BatteryTracker.Contracts.Services
{
    public interface ISettingsService
    {
        List<Tuple<string, string>> Languages { get; }

        bool EnableFullyChargedNotification { get; set; }

        bool EnableLowPowerNotification { get; set; }

        int LowPowerNotificationThreshold { get; set; }

        bool EnableHighPowerNotification { get; set; }

        int HighPowerNotificationThreshold { get; set; }

        ElementTheme Theme { get; set; }

        Tuple<string, string> Language { get; set; }

        bool RunAtStartup { get; set; }
    }
}
