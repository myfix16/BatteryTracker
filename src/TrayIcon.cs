using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.ApplicationServices;

namespace BatteryTracker
{
    internal class TrayIcon
    {
        #region Fields and Properties

        private NotifyIcon MainNotifyIcon { get; }

        /// <summary>
        /// The main entry of context menu.
        /// </summary>
        private ContextMenuStrip MainContextMenuStrip { get; }

        private ToolStripMenuItem ExitTerm { get; }

        private ToolStripMenuItem AutoStartTerm { get; }
        private bool _autoStartTermChecked = false;

        /// <summary>
        /// Remaining battery percentage.
        /// </summary>
        private int _powerPercentage;

        private const string PersonalizeSubKeyName = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";

        private string _colorState = "dark";

        //Startup registry key and value
        private const string StartupKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string StartupValue = "BatteryTracker";

        #endregion

        public TrayIcon()
        {
            MainNotifyIcon = new NotifyIcon();
            MainContextMenuStrip = new ContextMenuStrip();
            AutoStartTerm = new ToolStripMenuItem();
            ExitTerm = new ToolStripMenuItem();

            // initialize MainContextMenuStrip.
            MainContextMenuStrip.Items.Add(AutoStartTerm);
            MainContextMenuStrip.Items.Add(ExitTerm);

            // initialize AutoStartTerm
            using (var key = Registry.CurrentUser.OpenSubKey(StartupKey, false))
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

            // initialize ExitTerm.
            ExitTerm.Text = "Exit";
            ExitTerm.Click += new EventHandler(ToolStripMenuItemExit_Click);

            // Initialize MainNotifyIcon.
            MainNotifyIcon.ContextMenuStrip = MainContextMenuStrip;
            MainNotifyIcon.Visible = true;

            // Initialize color mode.
            DetectColorMode();
            UpdateIcon();

            // Listening to the change of color mode.
            var monitor = new RegistryMonitor(
                RegistryHive.CurrentUser,
                PersonalizeSubKeyName);

            monitor.RegChanged += (o, e) => { DetectColorMode(); UpdateIcon(); };

            // Listening to the change of battery percentage.
            PowerManager.BatteryLifePercentChanged += (o, e) => UpdateIcon();

            monitor.Start();
        }

        private void EnableStartup()
        {
            //Set the application to run at startup
            RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
            key.SetValue(StartupValue, Application.ExecutablePath);
        }

        private static void DisableStartup()
        {
            //Set the application to run at startup
            RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
            key.DeleteValue(StartupValue);
        }

        private void ToolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            MainNotifyIcon.Visible = false;
            MainNotifyIcon.Dispose();
            Environment.Exit(Environment.ExitCode);
        }

        /// <summary>
        /// Detects remaining percentage and updates the tray icon.
        /// </summary>
        private void UpdateIcon()
        {
            _powerPercentage = PowerManager.BatteryLifePercent;

            // ! Note that only public properties can be retrieved here by reflection.
            MainNotifyIcon.Icon = (Icon)typeof(ResourceIcon)
                .GetProperty($"{_colorState}_{_powerPercentage}")
                .GetValue(null, null);
        }

        /// <summary>
        /// A function that detect current color mode by look up in the corresponding registry key.
        /// </summary>
        private void DetectColorMode()
        {
            using var key = Registry.CurrentUser.OpenSubKey(PersonalizeSubKeyName);
            var info = int.TryParse(
                key.GetValue("SystemUsesLightTheme").ToString(),
                out int result);

            if (info)
            {
                _colorState = result switch
                {
                    0 => "dark",
                    1 => "light",
                    _ => throw new ArgumentException($"The argument {result} is not valid.")
                };
            }
        }
    }
}
