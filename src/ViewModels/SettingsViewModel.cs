using System.Collections.Generic;
using System.Windows.Input;
using BatteryTracker.Contracts.Services;
using BatteryTracker.Helpers;
using BatteryTracker.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Windows.System.Power;
using Windows.Globalization;
using Windows.UI.Popups;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;

namespace BatteryTracker.ViewModels;

public class SettingsViewModel : ObservableRecipient
{
    private ElementTheme _elementTheme;
    private bool _enableFullyChargedNotification;
    private bool _enableLowPowerNotification;
    private int _lowPowerNotificationThreshold;
    private Tuple<string, string> _language;
    private readonly string _appLanguage;
    private bool _languageChanged;
    private bool _enableAutostart;

    private const string FullyChargedNotificationSettingsKey = "EnableFullyChargedNotification";
    private const string LowPowerNotificationSettingsKey = "EnableLowPowerNotification";
    private const string LowPowerNotificationThresholdSettingsKey = "LowPowerNotificationThreshold";
    private const string LanguageSettingsKey = "language";
    private const string AutostartSettingsKey = "Autostart";

    private static readonly Dictionary<string, object> DefaultSettingsDict = new()
    {
        { FullyChargedNotificationSettingsKey, true },
        { LowPowerNotificationSettingsKey, true },
        { LowPowerNotificationThresholdSettingsKey, 25 },
        { LanguageSettingsKey, "English,en-US" },
        { AutostartSettingsKey, true },
    };

    // service reference
    private readonly BatteryIcon _batteryIcon;
    private readonly IAppNotificationService _notificationService;
    
    public ElementTheme ElementTheme
    {
        get => _elementTheme;
        private set => SetProperty(ref _elementTheme, value);
    }

    public ICommand SwitchThemeCommand { get; }

    public bool EnableFullyChargedNotification
    {
        get => _enableFullyChargedNotification;
        set
        {
            SetProperty(ref _enableFullyChargedNotification, value);
            _batteryIcon.EnableFullyChargedNotification = value;
            SettingsService.Set(FullyChargedNotificationSettingsKey, value);
        }
    }

    public bool EnableLowPowerNotification
    {
        get => _enableLowPowerNotification;
        set
        {
            SetProperty(ref _enableLowPowerNotification, value);
            _batteryIcon.EnableLowPowerNotification = value;
            SettingsService.Set(LowPowerNotificationSettingsKey, value);
        }
    }

    public int LowPowerNotificationThreshold
    {
        get => _lowPowerNotificationThreshold;
        set
        {
            SetProperty(ref _lowPowerNotificationThreshold, value);
            _batteryIcon.LowPowerNotificationThreshold = value;
            SettingsService.Set(LowPowerNotificationThresholdSettingsKey, value);
        }
    }

    public Tuple<string, string> Language
    {
        get => _language;
        set
        {
            // change app language
            ApplicationLanguages.PrimaryLanguageOverride = value.Item2;
            LanguageChanged = value.Item2 != _appLanguage;

            SetProperty(ref _language, value);
            SettingsService.Set(LanguageSettingsKey, $"{value.Item1},{value.Item2}");
        }
    }

    public bool LanguageChanged
    {
        get => _languageChanged;
        set => SetProperty(ref _languageChanged, value);
    }

    public ICommand RestartCommand { get; }

    public bool EnableAutostart
    {
        get => _enableAutostart;
        set
        {
            SetProperty(ref _enableAutostart, value);
            bool isRunAtStartup = AutoStartHelper.IsRunAtStartup().Result;
            switch (value)
            {
                case true when !isRunAtStartup:
                    Task.Run(AutoStartHelper.EnableStartup);
                    break;
                case false when isRunAtStartup:
                    Task.Run(AutoStartHelper.DisableStartup);
                    break;
            }
            SettingsService.Set(AutostartSettingsKey, value);
        }
    }

    public ICommand NotificationCommand { get; }

    public List<Tuple<string, string>> Languages { get; } = new()
    {
        new("English", "en-US"),
        new("简体中文", "zh-CN"),
    };

    public SettingsViewModel(BatteryIcon icon, IThemeSelectorService themeSelectorService, IAppNotificationService notificationService)
    {
        // initialize service references
        _batteryIcon = icon;
        IThemeSelectorService themeService = themeSelectorService;
        _notificationService = notificationService;

        _elementTheme = themeService.Theme;

        SwitchThemeCommand = new RelayCommand<ElementTheme?>(
            async (param) =>
            {
                if (param == null || ElementTheme == param.Value) return;
                ElementTheme = param.Value;
                await themeService.SetThemeAsync(param.Value);

                // set titlebar theme
                ElementTheme titleTheme = param.Value switch
                {
                    ElementTheme.Default => Application.Current.RequestedTheme == ApplicationTheme.Dark
                        ? ElementTheme.Dark
                        : ElementTheme.Light,
                    _ => param.Value
                };
                TitleBarHelper.UpdateTitleBar(titleTheme);
            });

        RestartCommand = new RelayCommand(() =>
        {
            Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
        });

        NotificationCommand = new RelayCommand(() =>
        {
            int percentage = PowerManager.RemainingChargePercent;
            TimeSpan remainingTime = PowerManager.RemainingDischargeTime;
            _notificationService.Show($"Lower power: {percentage}%. Estimated battery life: {remainingTime}");
        });

        IReadOnlyList<string> userLanguages = Windows.System.UserProfile.GlobalizationPreferences.Languages;
        DefaultSettingsDict[LanguageSettingsKey] = userLanguages[0];

        // initialize settings if necessary
        foreach (KeyValuePair<string, object> pair in DefaultSettingsDict)
        {
            if (!SettingsService.HasValue(pair.Key))
            {
                SettingsService.Set(pair.Key, pair.Value);
            }
        }

        // load settings values
        EnableFullyChargedNotification = (bool)SettingsService.Get(FullyChargedNotificationSettingsKey);
        EnableLowPowerNotification = (bool)SettingsService.Get(LowPowerNotificationSettingsKey);
        LowPowerNotificationThreshold = (int)SettingsService.Get(LowPowerNotificationThresholdSettingsKey);
        string[] languageParams = ((string)SettingsService.Get(LanguageSettingsKey)).Split(',');
        var loadedLanguage = Languages.Find(t => t.Item2 == languageParams[1]) ?? Languages[0];
        _appLanguage = loadedLanguage.Item2;
        Language = loadedLanguage;
        EnableAutostart = (bool)SettingsService.Get(AutostartSettingsKey);
    }
}
