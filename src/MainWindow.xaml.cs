using BatteryTracker.Helpers;
using H.NotifyIcon;
using WinUIEx;

namespace BatteryTracker;

public sealed partial class MainWindow : WindowEx
{
    public TaskbarIcon BatteryTrayIcon => TrayIcon;

    public MainWindow()
    {
        InitializeComponent();

        Content = null;
        // AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets\\Logo.ico"));
        Title = "AppDisplayName".Localized();
    }
}
