using BatteryTracker.Helpers;

namespace BatteryTracker.Tests.UnitTests.ModelsTests;

[TestClass]
public class LaunchHelperTest
{
    [TestMethod]
    public async Task TestLaunchHelperLaunchGitHubRepo()
    {
        bool result = await LaunchHelper.LaunchUriAsync(LaunchHelper.GitHubRepoUri);
        Assert.IsTrue(result);
        await Task.Delay(5000);
    }

    [TestMethod]
    public async Task TestLaunchHelperLaunchGitHubNewIssue()
    {
        bool result = await LaunchHelper.LaunchUriAsync(LaunchHelper.GitHubNewIssueUri);
        Assert.IsTrue(result);
        await Task.Delay(5000);
    }

    [TestMethod]
    public async Task TestLaunchHelperLaunchPrivacyStatement()
    {
        bool result = await LaunchHelper.LaunchUriAsync(LaunchHelper.PrivacyStatementUri);
        Assert.IsTrue(result);
        await Task.Delay(5000);
    }

    [TestMethod]
    public async Task TestLaunchHelperLaunchWindowsColorSettings()
    {
        bool result = await LaunchHelper.LaunchUriAsync(LaunchHelper.ColorsSettingsUri);
        Assert.IsTrue(result);
        await Task.Delay(5000);
    }

    [TestMethod]
    public async Task TestLaunchHelperLaunchEmailFeedback()
    {
        bool result = await LaunchHelper.LaunchUriAsync(LaunchHelper.EmailFeedbackUri);
        Assert.IsTrue(result);
        await Task.Delay(5000);
    }

    [TestMethod]
    public async Task TestLaunchHelperLaunchStoreRating()
    {
        bool result = await LaunchHelper.LaunchUriAsync(LaunchHelper.StoreRatingUri);
        Assert.IsTrue(result);
        await Task.Delay(5000);
    }

    // [UITestMethod]
    // public void UITestMethod()
    // {
    //     Assert.AreEqual(0, new Grid().ActualWidth);
    // }
}
