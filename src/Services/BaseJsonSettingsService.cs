using System.Collections.Generic;
using System.Text.Json;
using BatteryTracker.Contracts.Services;

namespace BatteryTracker.Services
{
    internal abstract class BaseJsonSettingsService
    {
        private readonly IDictionary<string, object> _settingsStorage;

        protected BaseJsonSettingsService(ISettingsStorageService settingsStorageService)
        {
            _settingsStorage = settingsStorageService.GetSettingsStorage();
        }

        protected T? Get<T>(string key, T? defaultValue)
        {
            if (string.IsNullOrEmpty(key))
            {
                return defaultValue;
            }

            if (!StorageHasValue(key))
            {
                return defaultValue;
            }
            return JsonSerializer.Deserialize<T>((string)_settingsStorage[key]);
        }

        protected void Set<T>(string key, T? value)
        {
            if (!string.IsNullOrEmpty(key))
            {
                StorageSetValue(key, value);
            }
        }

        protected object? StorageGetRawValue(string key) => StorageHasValue(key) ? _settingsStorage[key] : null;

        private bool StorageHasValue(string key) => _settingsStorage.ContainsKey(key);

        private void StorageSetValue<T>(string key, T value)
        {
            _settingsStorage[key] = JsonSerializer.Serialize(value);
        }
    }
}
