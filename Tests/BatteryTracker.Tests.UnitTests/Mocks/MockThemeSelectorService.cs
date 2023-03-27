using BatteryTracker.Contracts.Services;
using Microsoft.UI.Xaml;

namespace BatteryTracker.Tests.UnitTests.Mocks
{
    internal class MockThemeSelectorService : IThemeSelectorService
    {
        public ElementTheme Theme { get; private set; } = ElementTheme.Default;

        private readonly ISettingsService _settingsService;

        public MockThemeSelectorService(ISettingsService settingsService)
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
            _settingsService.Theme = theme;

            return Task.CompletedTask;
        }

        public Task SetRequestedThemeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
