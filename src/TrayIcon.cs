using System;
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
        ContextMenuStrip MainContextMenuStrip { get; }

        ToolStripMenuItem ExitTerm { get; }

        ToolStripMenuItem AutoStartTerm { get; }
        bool _autoStartTermChecked = false;

        ToolStripMenuItem LightModeTerm { get; }
        ToolStripMenuItem DarkModeTerm { get; }

        /// <summary>
        /// Remaining battery percentage.
        /// </summary>
        int _powerPercentage;

        const string PersonalizeSubKeyName = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";

        enum ColorState { Dark, Light }

        ColorState _colorState = ColorState.Dark;

        //Startup registry key and value
        const string StartupKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        const string StartupValue = "BatteryTracker";

        readonly RegistryMonitor _monitor = new RegistryMonitor(
            RegistryHive.CurrentUser,
            PersonalizeSubKeyName);

        #endregion

        public TrayIcon()
        {
            MainNotificationIcon = new NotifyIcon();
            MainContextMenuStrip = new ContextMenuStrip();
            AutoStartTerm = new ToolStripMenuItem();
            LightModeTerm = new ToolStripMenuItem();
            DarkModeTerm = new ToolStripMenuItem();
            ExitTerm = new ToolStripMenuItem();

            // initialize MainContextMenuStrip.
            MainContextMenuStrip.Items.Add(AutoStartTerm);
            MainContextMenuStrip.Items.Add(LightModeTerm);
            MainContextMenuStrip.Items.Add(DarkModeTerm);
            MainContextMenuStrip.Items.Add(ExitTerm);

            // initialize AutoStartTerm
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, false))
            {
                if (key.GetValue(StartupValue) != null) _autoStartTermChecked = true;
            }
            AutoStartTerm.Text = "Auto run at startup";
            AutoStartTerm.Checked = _autoStartTermChecked;
            AutoStartTerm.CheckOnClick = true;
            AutoStartTerm.CheckedChanged += (sender, args) =>
            {
                // Already checked, disable it. Otherwise, enable auto run at startup.
                if (_autoStartTermChecked) DisableStartup();
                else EnableStartup();
                _autoStartTermChecked ^= true;
            };

            // initialize Light and Dark mode term
            LightModeTerm.Text = "Light Mode";
            DarkModeTerm.Text = "Dark Mode";
            LightModeTerm.Checked = _colorState == ColorState.Light;
            DarkModeTerm.Checked = !LightModeTerm.Checked;
            LightModeTerm.Click += (sender, args) =>
            {
                if (_colorState == ColorState.Light) return;
                _colorState = ColorState.Light;
                LightModeTerm.Checked = true;
                DarkModeTerm.Checked = false;
                UpdateIcon();
            };
            DarkModeTerm.Click += (sender, args) =>
            {
                if (_colorState == ColorState.Dark) return;
                _colorState = ColorState.Dark;
                DarkModeTerm.Checked = true;
                LightModeTerm.Checked = false;
                UpdateIcon();
            };

            // initialize ExitTerm.
            ExitTerm.Text = "Exit";
            ExitTerm.Click += new EventHandler(ToolStripMenuItemExit_Click);

            // Initialize MainNotifyIcon.
            MainNotificationIcon.ContextMenuStrip = MainContextMenuStrip;
            MainNotificationIcon.Visible = true;

            // Initialize color mode.
            DetectColorMode();
            UpdateIcon();

            // Listening to the change of color mode.
            _monitor.RegChanged += (o, e) => { DetectColorMode(); UpdateIcon(); };

            // Listening to the change of battery percentage.
            PowerManager.BatteryLifePercentChanged += (o, e) => UpdateIcon();

            _monitor.Start();
        }

        ~TrayIcon()
        {
            _monitor.Stop();
        }

        void EnableStartup()
        {
            //Set the application to run at startup
            RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
            key.SetValue(StartupValue, Application.ExecutablePath);
        }

        static void DisableStartup()
        {
            //Set the application to run at startup
            RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
            key.DeleteValue(StartupValue);
        }

        void ToolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            MainNotificationIcon.Visible = false;
            MainNotificationIcon.Dispose();
            Environment.Exit(Environment.ExitCode);
        }

        /// <summary>
        /// Detects remaining percentage and updates the tray icon.
        /// </summary>
        void UpdateIcon()
        {
            _powerPercentage = PowerManager.BatteryLifePercent;

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
        void DetectColorMode()
        {
            using var key = Registry.CurrentUser.OpenSubKey(PersonalizeSubKeyName);
            var info = int.TryParse(
                key.GetValue("SystemUsesLightTheme").ToString(),
                out int result);

            if (info)
            {
                _colorState = result switch
                {
                    0 => ColorState.Dark,
                    1 => ColorState.Light,
                    _ => throw new ArgumentException($"The argument {result} is not valid.")
                };
            }
        }
    }
}
