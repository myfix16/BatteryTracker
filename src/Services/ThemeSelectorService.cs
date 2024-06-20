using BatteryTracker.Contracts.Services;
using BatteryTracker.Helpers;

namespace BatteryTracker.Services;

public sealed class ThemeSelectorService(ISettingsService settingsService) : IThemeSelectorService
{
    public ElementTheme Theme { get; private set; } = ElementTheme.Default;

    public async Task InitializeAsync()
    {
        Theme = settingsService.Theme;
        await Task.CompletedTask;
    }

    public async Task SetThemeAsync(ElementTheme theme)
    {
        Theme = theme;

        await SetRequestedThemeAsync();
        settingsService.Theme = theme;
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
}
