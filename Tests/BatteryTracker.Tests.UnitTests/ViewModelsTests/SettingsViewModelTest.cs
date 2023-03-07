using System.Diagnostics;
using BatteryTracker.Helpers;
using BatteryTracker.Services;
using BatteryTracker.Tests.UnitTests.Mocks;
using BatteryTracker.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BatteryTracker.Tests.UnitTests.ViewModelsTests;

// https://docs.microsoft.com/visualstudio/test/getting-started-with-unit-testing
// https://docs.microsoft.com/visualstudio/test/using-microsoft-visualstudio-testtools-unittesting-members-in-unit-tests
// https://docs.microsoft.com/visualstudio/test/run-unit-tests-with-test-explorer

[TestClass]
public class SettingsViewModelTest
{
    private readonly SettingsViewModel _viewModel;
    private readonly SettingsService _settingsService;

    public SettingsViewModelTest()
    {
        _settingsService = new SettingsService(new MockSettingsStorageService());

        _viewModel = new SettingsViewModel(
            new MockBatteryIcon(new MockAppNotificationService()),
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
        _viewModel.EnableFullyChargedNotification = isEnabled;
        Assert.AreEqual(isEnabled, (bool)_settingsService.Get(SettingsService.FullyChargedNotificationSettingsKey));
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void TestLowPowerNotificationSettingWroteToStorage(bool isEnabled)
    {
        // Low power notification
        _viewModel.EnableLowPowerNotification = isEnabled;
        Assert.AreEqual(isEnabled, (bool)_settingsService.Get(SettingsService.LowPowerNotificationSettingsKey));
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
        Assert.AreEqual(threshold, (int)_settingsService.Get(SettingsService.LowPowerNotificationThresholdSettingsKey));
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void TestHighPowerNotificationSettingWroteToStorage(bool isEnabled)
    {
        // High power notification
        _viewModel.EnableHighPowerNotification = isEnabled;
        Assert.AreEqual(isEnabled, (bool)_settingsService.Get(SettingsService.HighPowerNotificationSettingsKey));
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
        Assert.AreEqual(threshold,
            (int)_settingsService.Get(SettingsService.HighPowerNotificationThresholdSettingsKey));
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
            _viewModel.ElementTheme = theme;
        }
        catch { }

        Assert.AreEqual(theme,
            Enum.Parse<ElementTheme>((string)_settingsService.Get(SettingsService.AppThemeSettingsKey)));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    public void TestLanguageSettingWroteToStorage(int index)
    {
        // Language
        try
        {
            _viewModel.Language = SettingsService.Languages[index];
        }
        catch { }

        Assert.IsTrue(((string)_settingsService.Get(SettingsService.LanguageSettingsKey))
            .Contains(SettingsService.Languages[index].Item2));
    }
}
