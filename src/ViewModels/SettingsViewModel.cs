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
        get => SettingsService.EnableFullyChargedNotification;
        set
        {
            SetProperty(ref SettingsService.EnableFullyChargedNotification, value);
            _batteryIcon.EnableFullyChargedNotification = value;
            SettingsService.Set(SettingsService.FullyChargedNotificationSettingsKey, value);
        }
    }

    public bool EnableLowPowerNotification
    {
        get => SettingsService.EnableLowPowerNotification;
        set
        {
            SetProperty(ref SettingsService.EnableLowPowerNotification, value);
            _batteryIcon.EnableLowPowerNotification = value;
            SettingsService.Set(SettingsService.LowPowerNotificationSettingsKey, value);
        }
    }

    public int LowPowerNotificationThreshold
    {
        get => SettingsService.LowPowerNotificationThreshold;
        set
        {
            SetProperty(ref SettingsService.LowPowerNotificationThreshold, value);
            _batteryIcon.LowPowerNotificationThreshold = value;
            SettingsService.Set(SettingsService.LowPowerNotificationThresholdSettingsKey, value);
        }
    }

    public bool EnableHighPowerNotification
    {
        get => SettingsService.EnableHighPowerNotification;
        set
        {
            SetProperty(ref SettingsService.EnableHighPowerNotification, value);
            _batteryIcon.EnableHighPowerNotification = value;
            SettingsService.Set(SettingsService.HighPowerNotificationSettingsKey, value);
        }
    }

    public int HighPowerNotificationThreshold
    {
        get => SettingsService.HighPowerNotificationThreshold;
        set
        {
            SetProperty(ref SettingsService.HighPowerNotificationThreshold, value);
            _batteryIcon.HighPowerNotificationThreshold = value;
            SettingsService.Set(SettingsService.HighPowerNotificationThresholdSettingsKey, value);
        }
    }

    public Tuple<string, string> Language
    {
        get => SettingsService.Language;
        set
        {
            // change app language
            ApplicationLanguages.PrimaryLanguageOverride = value.Item2;
            LanguageChanged = value.Item2 != SettingsService.AppLanguage;

            SetProperty(ref SettingsService.Language, value);
            SettingsService.Set(SettingsService.LanguageSettingsKey, value.Item2);
        }
    }

    public bool LanguageChanged
    {
        get => _languageChanged;
        set => SetProperty(ref _languageChanged, value);
    }

    public bool EnableAutostart
    {
        get => SettingsService.EnableAutostart;
        set
        {
            SetProperty(ref SettingsService.EnableAutostart, value);
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
                        SettingsService.Set(SettingsService.AutostartSettingsKey, value);
                    }
                    else
                    {
                        // Log and revert the change
                        _logger.LogError($"Setting running at startup failed.");
                        SettingsService.Set(SettingsService.AutostartSettingsKey, !value);
                    }
                }
            });
        }
    }

    public List<Tuple<string, string>> Languages => SettingsService.Languages;

    public SettingsViewModel(BatteryIcon icon, IThemeSelectorService themeSelectorService, ILogger<SettingsViewModel> logger)
    {
        // initialize service references
        _batteryIcon = icon;
        _themeService = themeSelectorService;
        _logger = logger;

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
