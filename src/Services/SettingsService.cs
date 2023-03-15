using System.Collections.Generic;
using BatteryTracker.Contracts.Services;
using Windows.System.UserProfile;

namespace BatteryTracker.Services;

public sealed class SettingsService
{
    #region Setting Keys

    public const string FullyChargedNotificationSettingsKey = "EnableFullyChargedNotification";
    public const string LowPowerNotificationSettingsKey = "EnableLowPowerNotification";
    public const string LowPowerNotificationThresholdSettingsKey = "LowPowerNotificationThreshold";
    public const string HighPowerNotificationSettingsKey = "EnableHighPowerNotification";
    public const string HighPowerNotificationThresholdSettingsKey = "HighPowerNotificationThreshold";
    public const string LanguageSettingsKey = "language";
    public const string RunAtStartupSettingsKey = "Autostart";
    public const string AppThemeSettingsKey = "AppBackgroundRequestedTheme";

    #endregion

    #region Setting Values

    public static List<Tuple<string, string>> Languages { get; } = new()
    {
        new("English", "en-US"),
        new("简体中文", "zh-Hans"),
    };

    public bool EnableFullyChargedNotification;
    public bool EnableLowPowerNotification;
    public int LowPowerNotificationThreshold;
    public bool EnableHighPowerNotification;
    public int HighPowerNotificationThreshold;
    public Tuple<string, string> Language;
    public readonly string AppLanguage;
    public bool RunAtStartup;

    #endregion

    private readonly Dictionary<string, object> _defaultSettingsDict = new()
    {
        { FullyChargedNotificationSettingsKey, true },
        { LowPowerNotificationSettingsKey, true },
        { LowPowerNotificationThresholdSettingsKey, 25 },
        { HighPowerNotificationSettingsKey, true },
        { HighPowerNotificationThresholdSettingsKey, 80 },
        { LanguageSettingsKey, "en-US" },
        { RunAtStartupSettingsKey, true },
        { AppThemeSettingsKey, "Default" },
    };

    #region Methods

    public bool HasValue(string key) => _settingsStorage.ContainsKey(key);

    public void Set(string key, object value) => _settingsStorage[key] = value;

    public object Get(string key) => _settingsStorage[key];

    public bool TryGetValue(string key, out object? result)
    {
        bool found = _settingsStorage.TryGetValue(key, out object? value);
        if (found)
        {
            result = value;
            return true;
        }

        result = default;
        return false;
    }

    #endregion

    private readonly IDictionary<string, object> _settingsStorage;

    public SettingsService(ISettingsStorageService settingsStorageService)
    {
        _settingsStorage = settingsStorageService.GetSettingsStorage();

        // initialize settings if necessary
        IReadOnlyList<string> userLanguages = GlobalizationPreferences.Languages;
        _defaultSettingsDict[LanguageSettingsKey] = userLanguages[0];
        foreach (KeyValuePair<string, object> pair in _defaultSettingsDict)
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
        RunAtStartup = (bool)Get(RunAtStartupSettingsKey);
    }
}
