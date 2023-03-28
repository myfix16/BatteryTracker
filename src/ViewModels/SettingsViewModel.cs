using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using BatteryTracker.Activation;
using BatteryTracker.Contracts.Services;
using BatteryTracker.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Windows.Globalization;
using BatteryTracker.Models;
using Windows.System.UserProfile;
using BatteryTracker.Views;
using Microsoft.Windows.AppLifecycle;

namespace BatteryTracker.ViewModels;

public sealed class SettingsViewModel : ObservableRecipient
{
    public ElementTheme ElementTheme
    {
        get => _elementTheme;
        set
        {
            SetProperty(ref _elementTheme, value);
            SwitchThemeAsync(value).Wait();
        }
    }

    public ICommand RestartCommand { get; }

#if DEBUG
    // NotificationCommand- debug only
    public ICommand NotificationCommand { get; }
#endif

    private bool _enableFullyChargedNotification;

    public bool EnableFullyChargedNotification
    {
        get => _settingsService.EnableFullyChargedNotification;
        set
        {
            SetProperty(ref _enableFullyChargedNotification, value);
            _batteryIcon.EnableFullyChargedNotification = value;
            _settingsService.EnableFullyChargedNotification = value;
        }
    }

    private bool _enableLowPowerNotification;

    public bool EnableLowPowerNotification
    {
        get => _settingsService.EnableLowPowerNotification;
        set
        {
            SetProperty(ref _enableLowPowerNotification, value);
            _batteryIcon.EnableLowPowerNotification = value;
            _settingsService.EnableLowPowerNotification = value;
        }
    }

    private int _lowPowerNotificationThreshold;

    public int LowPowerNotificationThreshold
    {
        get => _settingsService.LowPowerNotificationThreshold;
        set
        {
            SetProperty(ref _lowPowerNotificationThreshold, value);
            _batteryIcon.LowPowerNotificationThreshold = value;
            _settingsService.LowPowerNotificationThreshold = value;
        }
    }

    private bool _enableHighPowerNotification;

    public bool EnableHighPowerNotification
    {
        get => _settingsService.EnableHighPowerNotification;
        set
        {
            SetProperty(ref _enableHighPowerNotification, value);
            _batteryIcon.EnableHighPowerNotification = value;
            _settingsService.EnableHighPowerNotification = value;
        }
    }

    private int _highPowerNotificationThreshold;

    public int HighPowerNotificationThreshold
    {
        get => _settingsService.HighPowerNotificationThreshold;
        set
        {
            SetProperty(ref _highPowerNotificationThreshold, value);
            _batteryIcon.HighPowerNotificationThreshold = value;
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
            // Check whether the actual language is changed
            if (value.LanguageId == string.Empty)
            {
                // Select system default
                newLanguageId = GlobalizationPreferences.Languages[0];
                LanguageChanged = newLanguageId != _appLanguageId;
            }
            else
            {
                LanguageChanged = value.LanguageId != _appLanguageId;
            }

            SetProperty(ref _language, value);
            _settingsService.Language = value;

            if (LanguageChanged)
            {
                ApplicationLanguages.PrimaryLanguageOverride = newLanguageId;
            }
        }
    }

    private bool _runAtStartup;

    public bool RunAtStartup
    {
        get => _settingsService.RunAtStartup;
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
                        _logger.LogInformation($"Set running at startup to: {value}");
                        _settingsService.RunAtStartup = value;
                    }
                    else
                    {
                        // Log and revert the change
                        _logger.LogError($"Setting running at startup failed.");
                        SetProperty(ref _runAtStartup, !value);
                    }
                }
            });
        }
    }

    public bool LanguageChanged
    {
        get => _languageChanged;
        private set => SetProperty(ref _languageChanged, value);
    }

    public IList<AppLanguageItem> Languages => _settingsService.Languages;

    #region Private fields

    private ElementTheme _elementTheme;

    private readonly string _appLanguageId;
    private bool _languageChanged;

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

        _elementTheme = _themeService.Theme;

        RestartCommand = new RelayCommand(() =>
        {
            AppInstance.Restart(LaunchActivationHandler.OpenSettingsCommandArg);
        });

#if DEBUG
        NotificationCommand = new RelayCommand(() =>
        {
            App.GetService<IAppNotificationService>().Show("test");
        });
#endif

        // todo: this line breaks the unit tests for SettingsViewModel
        _appLanguageId = ApplicationLanguages.PrimaryLanguageOverride;

        ReadSettingValues();
    }

    /// <summary>
    /// Read setting values from the settings service
    /// </summary>
    private void ReadSettingValues()
    {
        _enableFullyChargedNotification = _settingsService.EnableFullyChargedNotification;
        _enableLowPowerNotification = _settingsService.EnableLowPowerNotification;
        _lowPowerNotificationThreshold = _settingsService.LowPowerNotificationThreshold;
        _enableHighPowerNotification = _settingsService.EnableHighPowerNotification;
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
