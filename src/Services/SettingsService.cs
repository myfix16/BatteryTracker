using Windows.Foundation.Collections;
using Windows.Storage;

namespace BatteryTracker.Services;

public static class SettingsService
{
    private static readonly IPropertySet PropertySet = ApplicationData.Current.LocalSettings.Values;

    public static bool HasValue(string key) => PropertySet.ContainsKey(key);

    public static void Set(string key, object value) => PropertySet[key] = value;

    public static object Get(string key) => PropertySet[key];

    public static bool TryGetValue(string key, out object? result)
    {
        bool found = PropertySet.TryGetValue(key, out object? value);
        if (found)
        {
            result = value;
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }
}