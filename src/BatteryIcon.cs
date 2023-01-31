using System.Runtime.InteropServices;
using BatteryTracker.Contracts.Services;
using CommunityToolkit.WinUI;
using H.NotifyIcon;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.System.Power;
using Windows.UI.ViewManagement;
using Brush = Microsoft.UI.Xaml.Media.Brush;
using Color = Windows.UI.Color;

namespace BatteryTracker;

public partial class BatteryIcon : IDisposable
{
    private static readonly Brush White = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
    private static readonly Brush Black = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

    private TaskbarIcon? _trayIcon;
    private readonly UISettings _settings = new();
    private readonly IAppNotificationService _notificationService;

    private bool _isLowPower;

    private bool _trayIconEventsRegistered;

    private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    // notification settings
    public bool EnableLowPowerNotification;
    public int LowPowerNotificationThreshold;
    public bool EnableFullyChargedNotification;

    public BatteryIcon(IAppNotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task InitAsync(ResourceDictionary resources)
    {
        _trayIcon = (TaskbarIcon)resources["TrayIcon"];
        _trayIcon.ForceCreate(true);

        // init percentage and color
        await UpdateTrayIconPercent();
        OnThemeChanged(_settings, null);

        RegisterTrayIconEvents();

        // register display events
        PowerManager.DisplayStatusChanged += PowerManager_DisplayStatusChanged;
    }

    ~BatteryIcon()
    {
        Dispose();
    }

    public void Dispose()
    {
        UnregisterTrayIconEvents();
        PowerManager.DisplayStatusChanged -= PowerManager_DisplayStatusChanged;

        _trayIcon?.Dispose();
    }

    private void PowerManager_DisplayStatusChanged(object? sender, object _)
    {
        DisplayStatus displayStatus = PowerManager.DisplayStatus;
        switch (displayStatus)
        {
            case DisplayStatus.Off:
                if (_trayIconEventsRegistered) UnregisterTrayIconEvents();
                break;
            case DisplayStatus.On:
                if (!_trayIconEventsRegistered) RegisterTrayIconEvents();
                break;
            case DisplayStatus.Dimmed:
                break;
            default:
                throw new ArgumentOutOfRangeException($"Invalid display status: {displayStatus}");
        }
    }

    private void RegisterTrayIconEvents()
    {
        // register power events
        PowerManager.RemainingChargePercentChanged += OnRemainingChargePercentChanged;
        // register theme events
        _settings.ColorValuesChanged += OnThemeChanged;
        _trayIconEventsRegistered = true;
    }

    private void UnregisterTrayIconEvents()
    {
        // unregister power events
        PowerManager.RemainingChargePercentChanged -= OnRemainingChargePercentChanged;
        // unregister theme events
        _settings.ColorValuesChanged -= OnThemeChanged;
        _trayIconEventsRegistered = false;
    }

    private async void OnRemainingChargePercentChanged(object? _, object? eventArg)
    {
        int chargePercent = await UpdateTrayIconPercent();

        // push low power notification
        if (EnableLowPowerNotification && !_isLowPower && chargePercent < LowPowerNotificationThreshold)
        {
            _isLowPower = true;
            TimeSpan estimatedBatteryLife = PowerManager.RemainingDischargeTime;  // todo: COM Exception
            _notificationService.Show($"Lower power: {chargePercent}%. Estimated battery life: {estimatedBatteryLife}");
        }
        else if (chargePercent >= LowPowerNotificationThreshold)
        {
            _isLowPower = false;
        }

        // push fully charged notification
        if (EnableFullyChargedNotification && chargePercent == 100)
        {
            _notificationService.Show("The battery is fully charged⚡");
        }
    }

    private async ValueTask<int> UpdateTrayIconPercent()
    {
        int chargePercent = PowerManager.RemainingChargePercent;
        string newPercentText = chargePercent == 100 ? "F" : $"{chargePercent}";
        // Use a DispatcherQueue to execute UI related code on the main UI thread. Otherwise you may get an exception.
        await _dispatcherQueue.EnqueueAsync(() =>
        {
            if (_trayIcon?.GeneratedIcon != null)
            {
                _trayIcon.GeneratedIcon.Text = newPercentText;
            }
        });
        return chargePercent;
    }

    private async void OnThemeChanged(UISettings sender, object? _)
    {
        if (_trayIcon is null)
        {
            return;
        }

        await _dispatcherQueue.EnqueueAsync(() =>
        {
            Brush newForeground = ShouldSystemUseDarkMode() ? White : Black;

            if (_trayIcon.GeneratedIcon != null)
            {
                _trayIcon.GeneratedIcon.Foreground = newForeground;
            }
        });
    }

    [LibraryImport("UXTheme.dll", EntryPoint = "#138", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static unsafe partial bool ShouldSystemUseDarkMode();
}