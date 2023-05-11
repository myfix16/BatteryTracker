using System.Collections.Generic;
using BatteryTracker.Contracts.Models;
using BatteryTracker.Models;

namespace BatteryTracker.Contracts.Services
{
    public interface ISettingsService : IBatterySettings
    {
        IList<AppLanguageItem> Languages { get; }

        ElementTheme Theme { get; set; }

        AppLanguageItem Language { get; set; }

        bool RunAtStartup { get; set; }
    }
}
