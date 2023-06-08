using BatteryTracker.Contracts.Services;
using BatteryTracker.Tests.UnitTests.Mocks;
using BatteryTracker.Views;
using H.NotifyIcon;
using Microsoft.Extensions.Logging;
using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace BatteryTracker.Tests.UnitTests.ViewTests;

[TestClass]
public class BatteryIconTest
{
    private readonly BatteryIcon _batteryIcon;

    public BatteryIconTest()
    {
        IAppNotificationService appNotificationService = new MockAppNotificationService();
        ILogger<BatteryIcon> logger = new MockLogger<BatteryIcon>();

        var taskbarIcon = new TaskbarIcon()
        {
            Visibility = Visibility.Visible,
            NoLeftClickDelay = true,
            ContextMenuMode = ContextMenuMode.PopupMenu,
            ToolTipText = "Battery Tracker",
            IconSource = new GeneratedIconSource()
            {
                Text = "99",
                Foreground = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("Arial"),
                FontWeight = FontWeights.SemiBold,
                BackgroundType = BackgroundType.Rectangle,
            }
        };

        _batteryIcon = new BatteryIcon(appNotificationService, logger);
        _batteryIcon.InitAsync(taskbarIcon).Wait();
    }

    ~BatteryIconTest()
    {
        _batteryIcon.Dispose();
    }
#if DEBUG
    [UITestMethod]
    public void TestUpdateIconPercent()
    {
        for (int percent = 0; percent <= 100; percent++)
        {
            _batteryIcon.UpdateTrayIconPercent(percent);
            Thread.Sleep(100);
        }
    }
#endif
}
