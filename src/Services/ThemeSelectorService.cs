using BatteryTracker.Contracts.Services;
using BatteryTracker.Helpers;

namespace BatteryTracker.Services;

public sealed class ThemeSelectorService : IThemeSelectorService
{
    private readonly SettingsService _settingsService;

    public ElementTheme Theme { get; private set; } = ElementTheme.Default;

    public ThemeSelectorService(SettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public async Task InitializeAsync()
    {
        Theme = LoadThemeFromSettings();
        await Task.CompletedTask;
    }

    public async Task SetThemeAsync(ElementTheme theme)
    {
        Theme = theme;

        await SetRequestedThemeAsync();
        SaveThemeToSettings(theme);
    }

    public async Task SetRequestedThemeAsync()
    {
        if (App.MainWindow.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = Theme;

            TitleBarHelper.UpdateTitleBar(Theme);
        }

        await Task.CompletedTask;
    }

    private ElementTheme LoadThemeFromSettings()
    {
        bool found = _settingsService.TryGetValue(SettingsService.AppThemeSettingsKey, out object? result);
        if (found && result != null && Enum.TryParse((string)(result), out ElementTheme theme))
        {
            return theme;
        }

        SaveThemeToSettings(ElementTheme.Default);
        return ElementTheme.Default;
    }

    private void SaveThemeToSettings(ElementTheme theme)
    {
        _settingsService.Set(SettingsService.AppThemeSettingsKey, Enum.GetName(theme)!);
    }
}
