using System.Collections.Generic;
using BatteryTracker.Contracts.Services;
using Windows.Storage;

namespace BatteryTracker.Services
{
    internal sealed class AppLocalSettingsStorageService : ISettingsStorageService
    {
        public IDictionary<string, object> GetSettingsStorage()
        {
            return ApplicationData.Current.LocalSettings.Values;
        }
    }
}
