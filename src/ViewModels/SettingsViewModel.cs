using System.Collections.Generic;
using System.Windows.Input;
using BatteryTracker.Contracts.Services;
using BatteryTracker.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;

namespace BatteryTracker.ViewModels;

// todo: store settings persistently
public class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;
    private ElementTheme _elementTheme;
    private bool _enableFullyChargedNotification;
    private bool _enableLowPowerNotification;
    private int _lowPowerNotificationThreshold;

    public ElementTheme ElementTheme
    {
        get => _elementTheme;
        set => SetProperty(ref _elementTheme, value);
    }

    public ICommand SwitchThemeCommand { get; }

    public bool EnableFullyChargedNotification
    {
        get => _enableFullyChargedNotification;
        set => SetProperty(ref _enableFullyChargedNotification, value);
    }

    public bool EnableLowPowerNotification
    {
        get => _enableLowPowerNotification;
        set => SetProperty(ref _enableLowPowerNotification, value);
    }

    public int LowPowerNotificationThreshold
    {
        get => _lowPowerNotificationThreshold;
        set => SetProperty(ref _lowPowerNotificationThreshold, value);
    }

    public List<Tuple<string, string>> Languages { get; } = new()
    {
        new("English", "en-US"),
        new("简体中文", "zh-CN"),
    };

    public SettingsViewModel(IThemeSelectorService themeSelectorService)
    {
        _themeSelectorService = themeSelectorService;
        _elementTheme = _themeSelectorService.Theme;

        SwitchThemeCommand = new RelayCommand<ElementTheme?>(
            async (param) =>
            {
                if (param == null || ElementTheme == param.Value) return;
                ElementTheme newTheme = param.Value switch
                {
                    ElementTheme.Default => Application.Current.RequestedTheme == ApplicationTheme.Dark
                        ? ElementTheme.Dark
                        : ElementTheme.Light,
                    _ => param.Value
                };
                ElementTheme = newTheme;
                await _themeSelectorService.SetThemeAsync(newTheme);
                TitleBarHelper.UpdateTitleBar(newTheme);
            });
    }
}
