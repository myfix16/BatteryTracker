using BatteryTracker.Contracts.Services;
using Microsoft.UI.Xaml;

namespace BatteryTracker.Tests.UnitTests.Mocks
{
    internal class MockThemeSelectorService(ISettingsService settingsService) : IThemeSelectorService
    {
        public ElementTheme Theme { get; private set; } = ElementTheme.Default;

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public Task SetThemeAsync(ElementTheme theme)
        {
            Theme = theme;
            settingsService.Theme = theme;

            return Task.CompletedTask;
        }

        public Task SetRequestedThemeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
