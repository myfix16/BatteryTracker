using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using BatteryTracker.Activation;
using BatteryTracker.Contracts.Models;
using BatteryTracker.Contracts.Services;
using BatteryTracker.Helpers;
using BatteryTracker.Models;
using BatteryTracker.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Windows.AppLifecycle;
using Windows.Globalization;
using Windows.System.UserProfile;

namespace BatteryTracker.ViewModels;

public sealed class SettingsViewModel : ObservableRecipient, IBatterySettings
{
    private ElementTheme _appTheme;

    public ElementTheme AppTheme
    {
        get => _appTheme;
        set
        {
            SetProperty(ref _appTheme, value);
            SwitchThemeAsync(value).Wait();
        }
    }

    public ICommand RestartCommand { get; }

    public ICommand OpenTranslationUrlCommand { get; }

    public ICommand OpenWindowsColorSettingsCommand { get; }

#if DEBUG
    // debug only
    public ICommand NotificationCommand { get; }

    public ICommand TestTrayIconCommand { get; }

    private int _testTrayIconCount;
    public int TestTrayIconCount
    {
        get => _testTrayIconCount;
        set
        {
            SetProperty(ref _testTrayIconCount, value);
            _batteryIcon.UpdateTrayIconPercent(value);
        }
    }
#endif

    private bool _fullyChargedNotificationEnabled;

    public bool FullyChargedNotificationEnabled
    {
        get => _fullyChargedNotificationEnabled;
        set
        {
            SetProperty(ref _fullyChargedNotificationEnabled, value);
            _batteryIcon.Settings.FullyChargedNotificationEnabled = value;
            _settingsService.FullyChargedNotificationEnabled = value;
        }
    }

    private bool _lowPowerNotificationEnabled;

    public bool LowPowerNotificationEnabled
    {
        get => _lowPowerNotificationEnabled;
        set
        {
            SetProperty(ref _lowPowerNotificationEnabled, value);
            _batteryIcon.Settings.LowPowerNotificationEnabled = value;
            _settingsService.LowPowerNotificationEnabled = value;
        }
    }

    private int _lowPowerNotificationThreshold;

    public int LowPowerNotificationThreshold
    {
        get => _lowPowerNotificationThreshold;
        set
        {
            SetProperty(ref _lowPowerNotificationThreshold, value);
            _batteryIcon.Settings.LowPowerNotificationThreshold = value;
            _settingsService.LowPowerNotificationThreshold = value;
        }
    }

    private bool _highPowerNotificationEnabled;

    public bool HighPowerNotificationEnabled
    {
        get => _highPowerNotificationEnabled;
        set
        {
            SetProperty(ref _highPowerNotificationEnabled, value);
            _batteryIcon.Settings.HighPowerNotificationEnabled = value;
            _settingsService.HighPowerNotificationEnabled = value;
        }
    }

    private int _highPowerNotificationThreshold;

    public int HighPowerNotificationThreshold
    {
        get => _highPowerNotificationThreshold;
        set
        {
            SetProperty(ref _highPowerNotificationThreshold, value);
            _batteryIcon.Settings.HighPowerNotificationThreshold = value;
            _settingsService.HighPowerNotificationThreshold = value;
        }
    }

    private AppLanguageItem _language;

    public AppLanguageItem Language
    {
        get => _language;
        set
        {
            string newLanguageId = value.LanguageId;

            LanguageChanged = newLanguageId != _appLanguageId;
            SetProperty(ref _language, value);
            _settingsService.Language = value;

            if (LanguageChanged)
            {
                ApplicationLanguages.PrimaryLanguageOverride = value.LanguageId == string.Empty
                    ? GlobalizationPreferences.Languages[0]
                    : newLanguageId;

                _logger.LogInformation("Change language to {languageId}", value.LanguageId);
            }
        }
    }

    private bool _runAtStartup;

    public bool RunAtStartup
    {
        get => _runAtStartup;
        set
        {
            SetProperty(ref _runAtStartup, value);
            Task.Run(async () =>
            {
                bool isRunAtStartup = await StartupHelper.IsRunAtStartup();
                bool needChange = value != isRunAtStartup;
                if (needChange)
                {
                    bool success = value switch
                    {
                        true => await StartupHelper.EnableStartup(),
                        false => await StartupHelper.DisableStartup()
                    };

                    if (success)
                    {
                        _logger.LogInformation("Set running at startup to: {runAtStartupValue}", value);
                        _settingsService.RunAtStartup = value;
                    }
                    else
                    {
                        // Log and revert the change
                        _logger.LogError("Setting running at startup failed.");
                        SetProperty(ref _runAtStartup, !value);
                    }
                }
            });
        }
    }

    private bool _languageChanged;

    public bool LanguageChanged
    {
        get => _languageChanged;
        private set => SetProperty(ref _languageChanged, value);
    }

    public IList<AppLanguageItem> Languages => _settingsService.Languages;

    #region Private fields

    private readonly string _appLanguageId;

    // service reference
    private readonly BatteryIcon _batteryIcon;
    private readonly ILogger<SettingsViewModel> _logger;
    private readonly IThemeSelectorService _themeService;
    private readonly ISettingsService _settingsService;

    #endregion

    public SettingsViewModel(BatteryIcon icon, IThemeSelectorService themeSelectorService,
        ILogger<SettingsViewModel> logger, ISettingsService settingsService)
    {
        // initialize service references
        _batteryIcon = icon;
        _themeService = themeSelectorService;
        _logger = logger;
        _settingsService = settingsService;

        _appTheme = _themeService.Theme;

        RestartCommand = new RelayCommand(() =>
        {
            AppInstance.Restart(LaunchActivationHandler.OpenSettingsCommandArg);
        });

        OpenTranslationUrlCommand = new AsyncRelayCommand(async () =>
        {
            await LaunchHelper.LaunchUriAsync(LaunchHelper.TranslationUri);
        });

        OpenWindowsColorSettingsCommand = new AsyncRelayCommand(async () =>
        {
            await LaunchHelper.LaunchUriAsync(LaunchHelper.ColorsSettingsUri);
        });

        _appLanguageId = _settingsService.Language.LanguageId;
        ReadSettingValues();
        LanguageChanged = false;

#if DEBUG
        NotificationCommand = new RelayCommand(() =>
        {
            App.GetService<IAppNotificationService>().Show("test");
        });

        TestTrayIconCommand = new AsyncRelayCommand(async () =>
        {
            int curPercentage = _batteryIcon.ChargedPercent;

            for (int percentage = 0; percentage <= 100; percentage++)
            {
                _batteryIcon.UpdateTrayIconPercent(percentage);
                await Task.Delay(100);
            }

            _batteryIcon.UpdateTrayIconPercent(curPercentage);
        });
#endif
    }

    /// <summary>
    /// Read setting values from the settings service
    /// </summary>
    private void ReadSettingValues()
    {
        _fullyChargedNotificationEnabled = _settingsService.FullyChargedNotificationEnabled;
        _lowPowerNotificationEnabled = _settingsService.LowPowerNotificationEnabled;
        _lowPowerNotificationThreshold = _settingsService.LowPowerNotificationThreshold;
        _highPowerNotificationEnabled = _settingsService.HighPowerNotificationEnabled;
        _highPowerNotificationThreshold = _settingsService.HighPowerNotificationThreshold;
        _runAtStartup = _settingsService.RunAtStartup;
        Language = Languages.FirstOrDefault(l => l.LanguageId == _settingsService.Language.LanguageId)
                   ?? Languages[0];
    }

    private async Task SwitchThemeAsync(ElementTheme theme)
    {
        await _themeService.SetThemeAsync(theme);

        // set titlebar theme
        ElementTheme titleTheme = theme switch
        {
            ElementTheme.Default => Application.Current.RequestedTheme == ApplicationTheme.Dark
                ? ElementTheme.Dark
                : ElementTheme.Light,
            _ => theme
        };
        TitleBarHelper.UpdateTitleBar(titleTheme);
    }
}
