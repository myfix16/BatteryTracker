using System.Runtime.InteropServices;
using BatteryTracker.Helpers;
using CommunityToolkit.WinUI;
using H.NotifyIcon;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.System.Power;
using Windows.UI.ViewManagement;
using Brush = Microsoft.UI.Xaml.Media.Brush;
using Color = Windows.UI.Color;

namespace BatteryTracker
{
    public partial class BatteryIcon : IDisposable
    {
        private static readonly Brush White = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        private static readonly Brush Black = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        private TaskbarIcon? _trayIcon;
        private readonly UISettings _settings = new();

        private bool _isLowPower;

        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        // notification settings
        public bool EnableLowPowerNotification = true;
        public int LowPowerNotificationThreshold = 25;
        public bool EnableFullyChargedNotification = true;


        public void Init(ResourceDictionary resources)
        {
            _trayIcon = (TaskbarIcon)resources["TrayIcon"];
            _trayIcon.ForceCreate();

            // register power events
            PowerManager.RemainingChargePercentChanged += UpdateTrayIconPercent;

            // register theme events
            _settings.ColorValuesChanged += UpdateTrayIconTheme;
        }

        ~BatteryIcon()
        {
            Dispose();
        }

        public void Dispose()
        {
            PowerManager.RemainingChargePercentChanged -= UpdateTrayIconPercent;
            _settings.ColorValuesChanged -= UpdateTrayIconTheme;

            _trayIcon?.Dispose();
        }

        private async void UpdateTrayIconPercent(object? _, object eventArg)
        {
            if (_trayIcon is null)
            {
                return;
            }

            int chargePercent = PowerManager.RemainingChargePercent;
            string newPercentText = chargePercent == 100 ? "F" : $"{chargePercent}%";
            // Use a DispatcherQueue to execute UI related code on the main UI thread. Otherwise you may get an exception.
            await _dispatcherQueue.EnqueueAsync(() =>
            {
                if (_trayIcon.GeneratedIcon != null)
                {
                    _trayIcon.GeneratedIcon.Text = newPercentText;
                }
            });

            // push low power notification
            if (EnableLowPowerNotification && !_isLowPower && chargePercent < LowPowerNotificationThreshold)
            {
                _isLowPower = true;
                TimeSpan estimatedBatteryLife = PowerManager.RemainingDischargeTime;
                NotificationManager.PushMessage(
                    $"Lower power: {chargePercent}%. Estimated battery life: {estimatedBatteryLife}");
            }
            else if (chargePercent >= LowPowerNotificationThreshold)
            {
                _isLowPower = false;
            }

            // push fully charged notification
            if (EnableFullyChargedNotification && chargePercent == 100)
            {
                NotificationManager.PushMessage("The battery is fully charged⚡");
            }
        }

        private async void UpdateTrayIconTheme(UISettings sender, object args)
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
}
