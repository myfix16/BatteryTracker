using System.Collections.Generic;
using System.Windows.Input;
using BatteryTracker.Activation;
using BatteryTracker.Contracts.Services;
using BatteryTracker.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Windows.Globalization;

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

    private Tuple<string, string> _language;
    public Tuple<string, string> Language
    {
        get => _language;
        set
        {
            LanguageChanged = value.Item2 != _appLanguage;
            SetProperty(ref _language, value);
            _settingsService.Language = value;

            ApplicationLanguages.PrimaryLanguageOverride = value.Item2;
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

    public List<Tuple<string, string>> Languages => _settingsService.Languages;

    #region Private fields

    private ElementTheme _elementTheme;

    private readonly string _appLanguage;
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
            Microsoft.Windows.AppLifecycle.AppInstance.Restart(LaunchActivationHandler.OpenSettingsCommandArg);
        });

#if DEBUG
        NotificationCommand = new RelayCommand(() =>
        {
            App.GetService<IAppNotificationService>().Show("test");
        });
#endif

        ReadSettingValues();

        _appLanguage = Language.Item2;
    }

    private void ReadSettingValues()
    {
        _enableFullyChargedNotification = _settingsService.EnableFullyChargedNotification;
        _enableLowPowerNotification = _settingsService.EnableLowPowerNotification;
        _lowPowerNotificationThreshold = _settingsService.LowPowerNotificationThreshold;
        _enableHighPowerNotification = _settingsService.EnableHighPowerNotification;
        _highPowerNotificationThreshold = _settingsService.HighPowerNotificationThreshold;
        _language = _settingsService.Language;
        _runAtStartup = _settingsService.RunAtStartup;
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
