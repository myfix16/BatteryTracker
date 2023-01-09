using System;
using CommunityToolkit.WinUI.UI.Helpers;
using H.NotifyIcon;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
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
            if (_themeListener != null) _themeListener.ThemeChanged -= UpdateTrayIconTheme;
            _trayIcon?.Dispose();
        }

        internal void PushNotification(string title, string message)
        {
            var builder = new AppNotificationBuilder()
                .AddText("Send a message.")
                .AddTextBox("textBox");

            var notificationManager = AppNotificationManager.Default;
            notificationManager.Show(builder.BuildNotification());
        }

        private void UpdateTrayIconPercent(object? _, object eventArg)
        {
            if (_trayIcon is null) return;
            int chargePercent = PowerManager.RemainingChargePercent;
            _trayIcon.GeneratedIcon ??= new GeneratedIcon();
            _trayIcon.GeneratedIcon.Text = chargePercent == 100 ? "F" : $"{chargePercent}%";
            PushNotification("", "");
        }

        private void UpdateTrayIconTheme(ThemeListener sender)
        {
            if (_trayIcon is null) return;
            _trayIcon.GeneratedIcon ??= new GeneratedIcon();
            _trayIcon.GeneratedIcon.Foreground = sender.CurrentTheme == ApplicationTheme.Dark
                ? White : Black;
        }
    }
}
