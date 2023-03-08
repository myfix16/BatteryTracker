using System.Collections.Generic;
using System.Windows.Input;
using BatteryTracker.Activation;
using BatteryTracker.Contracts.Services;
using BatteryTracker.Helpers;
using BatteryTracker.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Windows.Globalization;

namespace BatteryTracker.ViewModels;

public class SettingsViewModel : ObservableRecipient
{
    private ElementTheme _elementTheme;

    private bool _languageChanged;

    // service reference
    private readonly BatteryIcon _batteryIcon;
    private readonly ILogger<SettingsViewModel> _logger;
    private readonly IThemeSelectorService _themeService;
    private readonly SettingsService _settingsService;

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

    public bool EnableFullyChargedNotification
    {
        get => _settingsService.EnableFullyChargedNotification;
        set
        {
            SetProperty(ref _settingsService.EnableFullyChargedNotification, value);
            _batteryIcon.EnableFullyChargedNotification = value;
            _settingsService.Set(SettingsService.FullyChargedNotificationSettingsKey, value);
        }
    }

    public bool EnableLowPowerNotification
    {
        get => _settingsService.EnableLowPowerNotification;
        set
        {
            SetProperty(ref _settingsService.EnableLowPowerNotification, value);
            _batteryIcon.EnableLowPowerNotification = value;
            _settingsService.Set(SettingsService.LowPowerNotificationSettingsKey, value);
        }
    }

    public int LowPowerNotificationThreshold
    {
        get => _settingsService.LowPowerNotificationThreshold;
        set
        {
            SetProperty(ref _settingsService.LowPowerNotificationThreshold, value);
            _batteryIcon.LowPowerNotificationThreshold = value;
            _settingsService.Set(SettingsService.LowPowerNotificationThresholdSettingsKey, value);
        }
    }

    public bool EnableHighPowerNotification
    {
        get => _settingsService.EnableHighPowerNotification;
        set
        {
            SetProperty(ref _settingsService.EnableHighPowerNotification, value);
            _batteryIcon.EnableHighPowerNotification = value;
            _settingsService.Set(SettingsService.HighPowerNotificationSettingsKey, value);
        }
    }

    public int HighPowerNotificationThreshold
    {
        get => _settingsService.HighPowerNotificationThreshold;
        set
        {
            SetProperty(ref _settingsService.HighPowerNotificationThreshold, value);
            _batteryIcon.HighPowerNotificationThreshold = value;
            _settingsService.Set(SettingsService.HighPowerNotificationThresholdSettingsKey, value);
        }
    }

    public Tuple<string, string> Language
    {
        get => _settingsService.Language;
        set
        {
            LanguageChanged = value.Item2 != _settingsService.AppLanguage;
            SetProperty(ref _settingsService.Language, value);
            _settingsService.Set(SettingsService.LanguageSettingsKey, value.Item2);

            ApplicationLanguages.PrimaryLanguageOverride = value.Item2;
        }
    }

    public bool LanguageChanged
    {
        get => _languageChanged;
        set => SetProperty(ref _languageChanged, value);
    }

    public bool RunAtStartup
    {
        get => _settingsService.RunAtStartup;
        set
        {
            SetProperty(ref _settingsService.RunAtStartup, value);
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
                        _settingsService.Set(SettingsService.RunAtStartupSettingsKey, value);
                    }
                    else
                    {
                        // Log and revert the change
                        _logger.LogError($"Setting running at startup failed.");
                        _settingsService.RunAtStartup = !value;
                    }
                }
            });
        }
    }

    public List<Tuple<string, string>> Languages => SettingsService.Languages;

    public SettingsViewModel(BatteryIcon icon, IThemeSelectorService themeSelectorService,
        ILogger<SettingsViewModel> logger, SettingsService settingsService)
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
