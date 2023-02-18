using System.Runtime.InteropServices;
using BatteryTracker.Helpers;
using WinRT;
using WinUIEx;

namespace BatteryTracker;

public sealed partial class MainWindow : WindowEx
{
    public MainWindow()
    {
        InitializeComponent();

        // AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        Content = null;
        Title = "AppDisplayName".GetLocalized();
    }
}
