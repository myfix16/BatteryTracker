using BatteryTracker.Contracts.Services;
using Microsoft.UI.Xaml;

namespace BatteryTracker.Tests.UnitTests.Mocks;

public class MockSettingsService : ISettingsService
{
    public List<Tuple<string, string>> Languages { get; }

    public bool EnableFullyChargedNotification { get; set; }

    public bool EnableLowPowerNotification { get; set; }

    public int LowPowerNotificationThreshold { get; set; }

    public bool EnableHighPowerNotification { get; set; }

    public int HighPowerNotificationThreshold { get; set; }

    public ElementTheme Theme { get; set; }

    public Tuple<string, string> Language { get; set; }

    public bool RunAtStartup { get; set; }

    public MockSettingsService()
    {
        Languages = new()
        {
            new("English", "en-US"),
            new("简体中文", "zh-Hans"),
        };
        Language = Languages[0];
    }
}
