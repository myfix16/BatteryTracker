using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BatteryTracker.Contracts.Services;
using Windows.Globalization;
using Windows.System.UserProfile;
using BatteryTracker.Models;

namespace BatteryTracker.Services;

internal sealed class SettingsService : BaseJsonSettingsService, ISettingsService
{
    #region Setting Keys

    private const string SettingVersionSettingsKey = "SettingsVersion";

    private const string EnableFullyChargedNotificationSettingsKey = "EnableFullyChargedNotification";
    private const string EnableLowPowerNotificationSettingsKey = "EnableLowPowerNotification";
    private const string LowPowerNotificationThresholdSettingsKey = "LowPowerNotificationThreshold";
    private const string EnableHighPowerNotificationSettingsKey = "EnableHighPowerNotification";
    private const string HighPowerNotificationThresholdSettingsKey = "HighPowerNotificationThreshold";
    private const string ThemeSettingsKey = "AppBackgroundRequestedTheme";
    private const string RunAtStartupSettingsKey = "Autostart";
    private const string LanguageSettingsKey = "language";

    #endregion

    #region Default settings

    private const bool EnableFullyChargedNotificationDefault = true;
    private const bool EnableLowPowerNotificationDefault = true;
    private const int LowPowerNotificationThresholdDefault = 25;
    private const bool EnableHighPowerNotificationDefault = true;
    private const int HighPowerNotificationThresholdDefault = 80;
    private const ElementTheme ThemeDefault = ElementTheme.Default;
    private const bool RunAtStartupDefault = true;
    private readonly AppLanguageItem _languageDefault;

    #endregion

    #region Setting Values

    public IList<AppLanguageItem> Languages { get; private set; }

    private bool _enableFullyChargedNotification;

    public bool EnableFullyChargedNotification
    {
        get => _enableFullyChargedNotification;
        set
        {
            _enableFullyChargedNotification = value;
            Set(EnableFullyChargedNotificationSettingsKey, value);
        }
    }

    private bool _enableLowPowerNotification;

    public bool EnableLowPowerNotification
    {
        get => _enableLowPowerNotification;
        set
        {
            _enableLowPowerNotification = value;
            Set(EnableLowPowerNotificationSettingsKey, value);
        }
    }

    private int _lowPowerNotificationThreshold;

    public int LowPowerNotificationThreshold
    {
        get => _lowPowerNotificationThreshold;
        set
        {
            _lowPowerNotificationThreshold = value;
            Set(LowPowerNotificationThresholdSettingsKey, value);
        }
    }

    private bool _enableHighPowerNotification;

    public bool EnableHighPowerNotification
    {
        get => _enableHighPowerNotification;
        set
        {
            _enableHighPowerNotification = value;
            Set(EnableHighPowerNotificationSettingsKey, value);
        }
    }

    private int _highPowerNotificationThreshold;

    public int HighPowerNotificationThreshold
    {
        get => _highPowerNotificationThreshold;
        set
        {
            _highPowerNotificationThreshold = value;
            Set(HighPowerNotificationThresholdSettingsKey, value);
        }
    }

    private ElementTheme _theme;

    public ElementTheme Theme
    {
        get => _theme;
        set
        {
            _theme = value;
            Set(ThemeSettingsKey, value);
        }
    }

    private AppLanguageItem _language;

    public AppLanguageItem Language
    {
        get => _language;
        set
        {
            _language = value;
            Set(LanguageSettingsKey, value);
        }
    }

    private bool _runAtStartup;

    public bool RunAtStartup
    {
        get => _runAtStartup;
        set
        {
            _runAtStartup = value;
            Set(RunAtStartupSettingsKey, value);
        }
    }

    #endregion

    public SettingsService(ISettingsStorageService settingsStorageService)
        : base(settingsStorageService)
    {
        // Add supported languages and set default language
        AddSupportedAppLanguages();
        string languageId = GlobalizationPreferences.Languages[0];
        _languageDefault = Languages!.FirstOrDefault(dl => dl.LanguageId == languageId) ?? Languages!.First();

        // Read setting values from storage
        string? settingsVersion = Get<string>(SettingVersionSettingsKey, null);
        if (settingsVersion is not App.SettingsVersion)
        {
            ConvertAndLoadOlderSettings();
        }
        else
        {
            LoadSettingValues();
        }
    }

    #region Methods

    private void AddSupportedAppLanguages()
    {
        Languages = ApplicationLanguages.ManifestLanguages
            .Append(string.Empty) // Add default language id
            .Select(language => new AppLanguageItem(language))
            .OrderBy(language => language.LanguageId is not "") // Default language on top
            .ThenBy(language => language.LanguageName)
            .ToList();
    }

    private void LoadSettingValues()
    {
        _enableFullyChargedNotification =
            Get(EnableFullyChargedNotificationSettingsKey, EnableFullyChargedNotificationDefault);
        _enableLowPowerNotification =
            Get(EnableLowPowerNotificationSettingsKey, EnableLowPowerNotificationDefault);
        _lowPowerNotificationThreshold =
            Get(LowPowerNotificationThresholdSettingsKey, LowPowerNotificationThresholdDefault);
        _enableHighPowerNotification =
            Get(EnableHighPowerNotificationSettingsKey, EnableHighPowerNotificationDefault);
        _highPowerNotificationThreshold =
            Get(HighPowerNotificationThresholdSettingsKey, HighPowerNotificationThresholdDefault);
        _theme = Get(ThemeSettingsKey, ThemeDefault);
        _runAtStartup = Get(RunAtStartupSettingsKey, RunAtStartupDefault);
        _language = Get(LanguageSettingsKey, _languageDefault)!;
    }

    /// <summary>
    /// If users' settings' version is lower than the current one, do necessary conversion.
    /// </summary>
    private void ConvertAndLoadOlderSettings()
    {
        // Read and convert old settings
        object? value;
        value = StorageGetRawValue(EnableFullyChargedNotificationSettingsKey);
        EnableFullyChargedNotification = value != null ? (bool)value : EnableFullyChargedNotificationDefault;
        value = StorageGetRawValue(EnableLowPowerNotificationSettingsKey);
        EnableLowPowerNotification = value != null ? (bool)value : EnableLowPowerNotificationDefault;
        value = StorageGetRawValue(EnableHighPowerNotificationSettingsKey);
        EnableHighPowerNotification = value != null ? (bool)value : EnableHighPowerNotificationDefault;
        value = StorageGetRawValue(LowPowerNotificationThresholdSettingsKey);
        LowPowerNotificationThreshold = value != null ? (int)value : LowPowerNotificationThresholdDefault;
        value = StorageGetRawValue(HighPowerNotificationThresholdSettingsKey);
        HighPowerNotificationThreshold = value != null ? (int)value : HighPowerNotificationThresholdDefault;
        value = StorageGetRawValue(ThemeSettingsKey);
        Theme = value != null ? Enum.Parse<ElementTheme>((string)value) : ThemeDefault;
        value = StorageGetRawValue(RunAtStartupSettingsKey);
        RunAtStartup = value != null ? (bool)value : RunAtStartupDefault;
        value = StorageGetRawValue(LanguageSettingsKey);
        var languageId = value != null ? (string)value : string.Empty;
        Language = Languages.FirstOrDefault(l => l.LanguageId.Contains(languageId) || languageId.Contains(l.LanguageId))
                   ?? _languageDefault;

        // Mark the new settings version
        Set(SettingVersionSettingsKey, App.SettingsVersion);
    }

    #endregion
}
