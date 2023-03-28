using System.Collections.Generic;
using BatteryTracker.Models;

namespace BatteryTracker.Contracts.Services
{
    public interface ISettingsService
    {
        IList<AppLanguageItem> Languages { get; }

        bool EnableFullyChargedNotification { get; set; }

        bool EnableLowPowerNotification { get; set; }

        int LowPowerNotificationThreshold { get; set; }

        bool EnableHighPowerNotification { get; set; }

        int HighPowerNotificationThreshold { get; set; }

        ElementTheme Theme { get; set; }

        AppLanguageItem Language { get; set; }

        bool RunAtStartup { get; set; }
    }
}
