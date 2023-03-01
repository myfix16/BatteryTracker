using System.Collections.Generic;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System.UserProfile;

namespace BatteryTracker.Services;

public static class SettingsService
{
    #region Setting Keys

    public const string FullyChargedNotificationSettingsKey = "EnableFullyChargedNotification";
    public const string LowPowerNotificationSettingsKey = "EnableLowPowerNotification";
    public const string LowPowerNotificationThresholdSettingsKey = "LowPowerNotificationThreshold";
    public const string HighPowerNotificationSettingsKey = "EnableHighPowerNotification";
    public const string HighPowerNotificationThresholdSettingsKey = "HighPowerNotificationThreshold";
    public const string LanguageSettingsKey = "language";
    public const string AutostartSettingsKey = "Autostart";

    #endregion

    #region Setting Values

    public static List<Tuple<string, string>> Languages { get; } = new()
    {
        new("English", "en-US"),
        new("简体中文", "zh-Hans"),
    };

    public static bool EnableFullyChargedNotification;
    public static bool EnableLowPowerNotification;
    public static int LowPowerNotificationThreshold;
    public static bool EnableHighPowerNotification;
    public static int HighPowerNotificationThreshold;
    public static Tuple<string, string> Language;
    public static readonly string AppLanguage;
    public static bool EnableAutostart;

    #endregion

    public static readonly Dictionary<string, object> DefaultSettingsDict = new()
    {
        { FullyChargedNotificationSettingsKey, true },
        { LowPowerNotificationSettingsKey, true },
        { LowPowerNotificationThresholdSettingsKey, 25 },
        { HighPowerNotificationSettingsKey, true },
        { HighPowerNotificationThresholdSettingsKey, 80 },
        { LanguageSettingsKey, "en-US" },
        { AutostartSettingsKey, true },
    };

    public static readonly IPropertySet PropertySet = ApplicationData.Current.LocalSettings.Values;

    #region Static Methods

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

        result = default;
        return false;
    }

    #endregion

    static SettingsService()
    {
        IReadOnlyList<string> userLanguages = GlobalizationPreferences.Languages;
        DefaultSettingsDict[LanguageSettingsKey] = userLanguages[0];

        // initialize settings if necessary
        foreach (KeyValuePair<string, object> pair in DefaultSettingsDict)
        {
            if (!HasValue(pair.Key))
            {
                Set(pair.Key, pair.Value);
            }
        }

        // load settings values
        EnableFullyChargedNotification = (bool)Get(FullyChargedNotificationSettingsKey);
        EnableLowPowerNotification = (bool)Get(LowPowerNotificationSettingsKey);
        LowPowerNotificationThreshold = (int)Get(LowPowerNotificationThresholdSettingsKey);
        EnableHighPowerNotification = (bool)Get(HighPowerNotificationSettingsKey);
        HighPowerNotificationThreshold = (int)Get(HighPowerNotificationThresholdSettingsKey);
        string languageParam = (string)Get(LanguageSettingsKey);
        Tuple<string, string> loadedLanguage = Languages.Find(t => languageParam.Contains(t.Item2)) ?? Languages[0];
        AppLanguage = loadedLanguage.Item2;
        Language = loadedLanguage;
        EnableAutostart = (bool)Get(AutostartSettingsKey);
    }
}
