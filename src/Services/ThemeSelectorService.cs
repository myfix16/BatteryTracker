using BatteryTracker.Contracts.Services;
using BatteryTracker.Helpers;

namespace BatteryTracker.Services;

public class ThemeSelectorService : IThemeSelectorService
{
    private const string SettingsKey = "AppBackgroundRequestedTheme";

    public ElementTheme Theme { get; private set; } = ElementTheme.Default;

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

    private static ElementTheme LoadThemeFromSettings()
    {
        bool found = SettingsService.TryGetValue(SettingsKey, out object? result);
        if (found && result != null && Enum.TryParse((string)(result), out ElementTheme theme))
        {
            return theme;
        }

        SaveThemeToSettings(ElementTheme.Default);
        return ElementTheme.Default;
    }

    private static void SaveThemeToSettings(ElementTheme theme)
    {
        SettingsService.Set(SettingsKey, Enum.GetName(theme)!);
    }
}
