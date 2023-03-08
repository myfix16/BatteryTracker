using System.Collections.Generic;

namespace BatteryTracker.Contracts.Services
{
    public interface ISettingsStorageService
    {
        IDictionary<string, object> GetSettingsStorage();
    }
}
