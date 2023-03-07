using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatteryTracker.Contracts.Services;
using Windows.Storage;

namespace BatteryTracker.Services
{
    internal class AppLocalSettingsStorageService: ISettingsStorageService
    {
        public IDictionary<string, object> GetSettingsStorage()
        {
            return ApplicationData.Current.LocalSettings.Values;
        }
    }
}
