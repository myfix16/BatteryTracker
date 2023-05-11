using BatteryTracker.Contracts.Services;
using BatteryTracker.Tests.UnitTests.Mocks;
using BatteryTracker.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;

namespace BatteryTracker.Tests.UnitTests.ViewModelsTests;

// https://docs.microsoft.com/visualstudio/test/getting-started-with-unit-testing
// https://docs.microsoft.com/visualstudio/test/using-microsoft-visualstudio-testtools-unittesting-members-in-unit-tests
// https://docs.microsoft.com/visualstudio/test/run-unit-tests-with-test-explorer

[TestClass]
public class SettingsViewModelTest
{
    private readonly SettingsViewModel _viewModel;
    private readonly ISettingsService _settingsService;

    public SettingsViewModelTest()
    {
        ILoggerFactory loggerFactory = new LoggerFactory();

        _settingsService = new MockSettingsService();

        _viewModel = new SettingsViewModel(
            new MockBatteryIcon(new MockAppNotificationService(), new Logger<MockBatteryIcon>(loggerFactory)),
            new MockThemeSelectorService(_settingsService),
            new MockLogger<SettingsViewModel>(),
            _settingsService
        );
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void TestFullyChargedNotificationSettingWroteToStorage(bool isEnabled)
    {
        // Fully charged notification
        _viewModel.FullyChargedNotificationEnabled = isEnabled;
        Assert.AreEqual(isEnabled, _settingsService.FullyChargedNotificationEnabled);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void TestLowPowerNotificationSettingWroteToStorage(bool isEnabled)
    {
        // Low power notification
        _viewModel.LowPowerNotificationEnabled = isEnabled;
        Assert.AreEqual(isEnabled, _settingsService.LowPowerNotificationEnabled);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(25)]
    [DataRow(50)]
    [DataRow(75)]
    [DataRow(100)]
    public void TestLowPowerThresholdSettingWroteToStorage(int threshold)
    {
        // Low power threshold
        _viewModel.LowPowerNotificationThreshold = threshold;
        Assert.AreEqual(threshold, _settingsService.LowPowerNotificationThreshold);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void TestHighPowerNotificationSettingWroteToStorage(bool isEnabled)
    {
        // High power notification
        _viewModel.HighPowerNotificationEnabled = isEnabled;
        Assert.AreEqual(isEnabled, _settingsService.HighPowerNotificationEnabled);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(25)]
    [DataRow(50)]
    [DataRow(75)]
    [DataRow(100)]
    public void TestHighPowerThresholdSettingWroteToStorage(int threshold)
    {
        // High power threshold
        _viewModel.HighPowerNotificationThreshold = threshold;
        Assert.AreEqual(threshold, _settingsService.HighPowerNotificationThreshold);
    }

    [TestMethod]
    [DataRow(ElementTheme.Default)]
    [DataRow(ElementTheme.Dark)]
    [DataRow(ElementTheme.Light)]
    public void TestAppThemeSettingWroteToStorage(ElementTheme theme)
    {
        // App theme
        try
        {
            _viewModel.AppTheme = theme;
        }
        catch { }

        Assert.AreEqual(theme, _settingsService.Theme);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    public void TestLanguageSettingWroteToStorage(int index)
    {
        // Language
        try
        {
            _viewModel.Language = _settingsService.Languages[index];
        }
        catch { }

        Assert.IsTrue((_settingsService.Language.LanguageId.Contains(_settingsService.Languages[index].LanguageId)));
    }
}
