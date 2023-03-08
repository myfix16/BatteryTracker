using BatteryTracker.Contracts.Services;
using BatteryTracker.Services;
using Microsoft.UI.Xaml;

namespace BatteryTracker.Tests.UnitTests.Mocks
{
    internal class MockThemeSelectorService : IThemeSelectorService
    {
        public ElementTheme Theme { get; private set; } = ElementTheme.Default;

        private readonly SettingsService _settingsService;

        public MockThemeSelectorService(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public Task SetThemeAsync(ElementTheme theme)
        {
            Theme = theme;
            _settingsService.Set(SettingsService.AppThemeSettingsKey, Enum.GetName(theme)!);

            return Task.CompletedTask;
        }

        public Task SetRequestedThemeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
