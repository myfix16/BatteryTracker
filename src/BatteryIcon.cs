using BatteryTracker.Helpers;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Helpers;
using H.NotifyIcon;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.System.Power;
using Brush = Microsoft.UI.Xaml.Media.Brush;
using Color = Windows.UI.Color;

namespace BatteryTracker
{
    internal class BatteryIcon : IDisposable
    {
        private static readonly Brush White = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        private static readonly Brush Black = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        private TaskbarIcon? _trayIcon;
        private ThemeListener? _themeListener;

        private bool _isLowPower;

        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        public void Init(ResourceDictionary resources)
        {
            _trayIcon = (TaskbarIcon)resources["TrayIcon"];
            _trayIcon.ForceCreate();

            // register power events
            PowerManager.RemainingChargePercentChanged += UpdateTrayIconPercent;

            // register theme events
            _themeListener ??= new ThemeListener();
            _themeListener.ThemeChanged += UpdateTrayIconTheme;
        }

        ~BatteryIcon()
        {
            Dispose();
        }

        public void Dispose()
        {
            PowerManager.RemainingChargePercentChanged -= UpdateTrayIconPercent;
            if (_themeListener != null)
            {
                _themeListener.ThemeChanged -= UpdateTrayIconTheme;
            }

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
            await dispatcherQueue.EnqueueAsync(() =>
            {
                _trayIcon.GeneratedIcon.Text = newPercentText;
            });

            // NotificationManager.PushMessage($"Percentage: {chargePercent}");

            // push low power notification
            if (!_isLowPower && chargePercent < 25)
            {
                _isLowPower = true;
                TimeSpan estimatedBatteryLife = PowerManager.RemainingDischargeTime;
                NotificationManager.PushMessage(
                    $"Lower power: {chargePercent}%. Estimated battery life: {estimatedBatteryLife}");
            }
            else if (chargePercent >= 25)
            {
                _isLowPower = false;
            }

            // push fully charged notification
            if (chargePercent == 100)
            {
                NotificationManager.PushMessage("The battery is fully charged⚡");
            }
        }

        private async void UpdateTrayIconTheme(ThemeListener sender)
        {
            if (_trayIcon is null)
            {
                return;
            }

            Brush newForeground = sender.CurrentTheme == ApplicationTheme.Dark ? White : Black;
            await dispatcherQueue.EnqueueAsync(() =>
            {
                _trayIcon.GeneratedIcon.Foreground = newForeground;
            });

            NotificationManager.PushMessage($"Theme changed to: {sender.CurrentThemeName}");
        }
    }
}
