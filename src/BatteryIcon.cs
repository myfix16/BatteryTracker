using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using BatteryTracker.Contracts.Services;
using BatteryTracker.Helpers;
using CommunityToolkit.WinUI;
using H.NotifyIcon;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.System.Power;
using Windows.UI.ViewManagement;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<BatteryIcon> _logger;

    private int _chargedPercent;

    private bool _isLowPower;
    private bool _isHighPower;

    private bool _trayIconEventsRegistered;

    private DispatcherQueue? _dispatcherQueue;

    private static readonly Dictionary<double, int> DpiFontSizeMap = new()
    {
        { 1.0, 82 },
        { 1.25, 66 },
        { 1.5, 55 },
        { 1.75, 45 },
        { 2.0, 40 },
        { 2.25, 36 },
        { 2.5, 33 },
        { 3, 27 },
        { 3.5, 23 },
    };

    // todo: [Code quality] change public fields to properties
    // notification settings
    public bool EnableLowPowerNotification;

    public int LowPowerNotificationThreshold
    {
        get => _lowPowerNotificationThreshold;
        set
        {
            _lowPowerNotificationThreshold = value;
            _isLowPower = false;
        }
    }

    public bool EnableHighPowerNotification;

    public int HighPowerNotificationThreshold
    {
        get => _highPowerNotificationThreshold;
        set
        {
            _highPowerNotificationThreshold = value;
            _isHighPower = false;
        }
    }

    public bool EnableFullyChargedNotification;

    private int _lowPowerNotificationThreshold;
    private int _highPowerNotificationThreshold;

    public BatteryIcon(IAppNotificationService notificationService, ILogger<BatteryIcon> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task InitAsync(TaskbarIcon icon)
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        _trayIcon = icon;
        _trayIcon.ForceCreate(true);

        // init percentage and color
        await UpdateTrayIconPercent();
        await UpdateTrayIconTheme();

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

        GC.SuppressFinalize(this);
    }

    public async Task AdaptToDpiChange(double rastScale)
    {
        double scale = DpiFontSizeMap.Keys.MinBy(d => Math.Abs(d - rastScale));
        await _dispatcherQueue!.EnqueueAsync(() =>
        {
            if (_trayIcon?.GeneratedIcon != null)
            {
                _trayIcon.GeneratedIcon.FontSize = DpiFontSizeMap[scale];
            }
        });
    }

    private async void PowerManager_DisplayStatusChanged(object? sender, object _)
    {
        DisplayStatus displayStatus = PowerManager.DisplayStatus;
        switch (displayStatus)
        {
            case DisplayStatus.Off:
                if (_trayIconEventsRegistered)
                {
                    UnregisterTrayIconEvents();
                    _logger.LogInformation("Display off, unregister power events");
                }
                break;
            case DisplayStatus.On:
                if (!_trayIconEventsRegistered)
                {
                    RegisterTrayIconEvents();
                    _logger.LogInformation("Display on, register power events");
                }
                await UpdateTrayIconPercent();
                await UpdateTrayIconTheme();
                break;
            case DisplayStatus.Dimmed:
                break;
            default:
                _logger.LogWarning($"Invalid display status: {displayStatus}");
                break;
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

        // ! note: cannot add display status events here
    }

    private async void OnRemainingChargePercentChanged(object? _, object? eventArg)
    {
        _chargedPercent = await UpdateTrayIconPercent();

        // push low power notification
        if (EnableLowPowerNotification && !_isLowPower && _chargedPercent <= LowPowerNotificationThreshold)
        {
            _isLowPower = true;
            _notificationService.Show($"{"LowPowerMessage".Localized()}: {_chargedPercent}%");
        }
        else if (_chargedPercent > LowPowerNotificationThreshold)
        {
            _isLowPower = false;
        }

        // push high power notification
        if (EnableHighPowerNotification && !_isHighPower && _chargedPercent >= HighPowerNotificationThreshold)
        {
            _isHighPower = true;
            _notificationService.Show($"{"HighPowerMessage".Localized()}: {_chargedPercent}%");
        }
        else if (_chargedPercent < HighPowerNotificationThreshold)
        {
            _isHighPower = false;
        }

        // push fully charged notification
        if (EnableFullyChargedNotification && _chargedPercent == 100)
        {
            _notificationService.Show($"{"FullyChargedMessage".Localized()}⚡");
        }
    }

    private async Task<int> UpdateTrayIconPercent()
    {
        _chargedPercent = PowerManager.RemainingChargePercent;
        string newPercentText = _chargedPercent == 100 ? "F" : $"{_chargedPercent}";
        // Use a DispatcherQueue to execute UI related code on the main UI thread. Otherwise you may get an exception.
        await _dispatcherQueue!.EnqueueAsync(() =>
        {
            if (_trayIcon?.GeneratedIcon != null)
            {
                _trayIcon.GeneratedIcon.Text = newPercentText;
            }
        });
        return _chargedPercent;
    }

    private async void OnThemeChanged(UISettings sender, object? _)
    {
        await UpdateTrayIconTheme();
    }

    private async Task UpdateTrayIconTheme()
    {
        if (_trayIcon is null)
        {
            return;
        }

        await _dispatcherQueue!.EnqueueAsync(() =>
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