﻿using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.ApplicationServices;

namespace BatteryTracker
{
    class TrayIcon
    {
        #region Fields and Properties

        NotifyIcon MainNotificationIcon { get; }

        /// <summary>
        /// The main entry of context menu.
        /// </summary>
        ContextMenu MainContextMenuStrip { get; }

        MenuItem ExitTerm { get; }

        MenuItem AutoStartTerm { get; }

        MenuItem LightModeTerm { get; }
        MenuItem DarkModeTerm { get; }

        int _powerPercentage = PowerManager.BatteryLifePercent;

        const string PersonalizeSubKeyName = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";

        const string StartupValue = "BatteryTracker";

        enum ColorState
        {
            Dark,
            Light
        }

        ColorState _colorState = DetectColorMode();

        readonly RegistryMonitor _monitor = new RegistryMonitor(
            RegistryHive.CurrentUser,
            PersonalizeSubKeyName);

        #endregion

        public TrayIcon()
        {
            AutoStartTerm = new MenuItem
            {
                Text = "Auto run at startup",
                Checked = AutoStartHelper.IsRunAtStartup(StartupValue)
            };
            LightModeTerm = new MenuItem { Text = "Light mode", Checked = _colorState == ColorState.Light };
            DarkModeTerm = new MenuItem { Text = "Dark mode", Checked = !LightModeTerm.Checked };
            ExitTerm = new MenuItem { Text = "Exit" };
            MainContextMenuStrip = new ContextMenu
            { MenuItems = { AutoStartTerm, LightModeTerm, DarkModeTerm, ExitTerm } };
            MainNotificationIcon = new NotifyIcon
            {
                ContextMenu = MainContextMenuStrip,
                Visible = true,
                Icon = (Icon)typeof(ResourceIcon)
                    .GetProperty($"{_colorState}_{_powerPercentage}")
                    .GetValue(null, null)
            };

            // add event handlers
            AutoStartTerm.Click += (sender, args) =>
            {
                // Already checked, disable it. Otherwise, enable auto run at startup.
                if (AutoStartTerm.Checked) AutoStartHelper.DisableStartup(StartupValue);
                else AutoStartHelper.EnableStartup(StartupValue);
                AutoStartTerm.Checked ^= true;
            };

            LightModeTerm.Click += (sender, args) =>
            {
                if (_colorState == ColorState.Light) return;
                _colorState = ColorState.Light;
                LightModeTerm.Checked = true;
                DarkModeTerm.Checked = false;
                UpdateIconColor();
            };
            DarkModeTerm.Click += (sender, args) =>
            {
                if (_colorState == ColorState.Dark) return;
                _colorState = ColorState.Dark;
                DarkModeTerm.Checked = true;
                LightModeTerm.Checked = false;
                UpdateIconColor();
            };

            ExitTerm.Click += ApplicationExit;

            // Listening to the change of color mode.
            _monitor.RegChanged += (o, e) =>
            {
                if (_colorState == DetectColorMode()) return;
                _colorState = (ColorState)((int)_colorState ^ 1);
                UpdateIconColor();
            };

            // Listening to the change of battery percentage.
            PowerManager.BatteryLifePercentChanged += (o, e) => UpdateIconPercentage();

            _monitor.Start();
        }

        ~TrayIcon()
        {
            _monitor.Stop();
        }

        void ApplicationExit(object sender, EventArgs e)
        {
            MainNotificationIcon.Dispose();
            Environment.Exit(Environment.ExitCode);
        }

        /// <summary>
        /// Detects remaining percentage and updates the tray icon.
        /// </summary>
        void UpdateIconPercentage()
        {
            _powerPercentage = PowerManager.BatteryLifePercent;

            // ! Note that only public properties can be retrieved here by reflection.
            MainNotificationIcon.Icon = (Icon)typeof(ResourceIcon)
                .GetProperty($"{_colorState}_{_powerPercentage}")
                .GetValue(null, null);

            if (_powerPercentage < 25)
            {
                NotificationHelper.PushLowPowerNotification(
                    _powerPercentage,
                    SystemInformation.PowerStatus.BatteryLifeRemaining);
            }
            else if (_powerPercentage == 100)
            {
                NotificationHelper.PushChargedNotification();
            }
        }

        void UpdateIconColor()
        {
            // ! Note that only public properties can be retrieved here by reflection.
            MainNotificationIcon.Icon = (Icon)typeof(ResourceIcon)
                .GetProperty($"{_colorState}_{_powerPercentage}")
                .GetValue(null, null);
            LightModeTerm.Checked = _colorState == ColorState.Light;
            DarkModeTerm.Checked = !LightModeTerm.Checked;
        }

        /// <summary>
        /// A function that detect current color mode by look up in the corresponding registry key.
        /// </summary>
        static ColorState DetectColorMode()
        {
            using RegistryKey key = Registry.CurrentUser.OpenSubKey(PersonalizeSubKeyName);
            bool info = int.TryParse(key.GetValue("SystemUsesLightTheme").ToString(), out int result);

            return (ColorState)result;
        }
    }
}