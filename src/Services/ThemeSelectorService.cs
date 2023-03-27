using BatteryTracker.Contracts.Services;
using BatteryTracker.Helpers;

namespace BatteryTracker.Services;

public sealed class ThemeSelectorService : IThemeSelectorService
{
    private readonly ISettingsService _settingsService;

    public ElementTheme Theme { get; private set; } = ElementTheme.Default;

    public ThemeSelectorService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public async Task InitializeAsync()
    {
        Theme = _settingsService.Theme;
        await Task.CompletedTask;
    }

    public async Task SetThemeAsync(ElementTheme theme)
    {
        Theme = theme;

        await SetRequestedThemeAsync();
        _settingsService.Theme = theme;
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
