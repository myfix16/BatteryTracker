using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.ApplicationServices;
using RegistryUtils;

namespace BatteryTrackerFramework
{
    class TrayIcon
    {
        #region Fields and Properties

        NotifyIcon MainNotifyIcon { get; }

        /// <summary>
        /// The main entry of context menu.
        /// </summary>
        ContextMenuStrip MainContextMenuStrip { get; }

        ToolStripMenuItem ExitTerm { get; }

        /// <summary>
        /// Remaining battery percentage.
        /// </summary>
        int powerPercentage;

        const string subKeyName = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";

        string colorState = "dark";

        #endregion

        public TrayIcon()
        {
            MainNotifyIcon = new NotifyIcon();
            MainContextMenuStrip = new ContextMenuStrip();
            ExitTerm = new ToolStripMenuItem();

            // initialize MainContextMenuStrip.
            //MainContextMenuStrip.Items.AddRange(new ToolStripItem[] { ExitTerm });
            MainContextMenuStrip.Items.Add(ExitTerm);

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
                subKeyName);

            monitor.RegChanged += (o, e) => { DetectColorMode(); UpdateIcon(); };

            // Listening to the change of battery percentage.
            PowerManager.BatteryLifePercentChanged += (o, e) => UpdateIcon();

            monitor.Start();
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
            powerPercentage = PowerManager.BatteryLifePercent;

            // ! Note that only public properties can be retrieved here by reflection.
            MainNotifyIcon.Icon = (Icon)typeof(ResourceIcon)
                .GetProperty($"{colorState}_{powerPercentage}")
                .GetValue(null, null);
        }

        /// <summary>
        /// A function that detect current color mode by look up in the corresponding registry key.
        /// </summary>
        private void DetectColorMode()
        {
            using var key = Registry.CurrentUser.OpenSubKey(subKeyName);
            var info = int.TryParse(
                key.GetValue("SystemUsesLightTheme").ToString(),
                out int result);

            if (info)
            {
                colorState = result switch
                {
                    0 => "dark",
                    1 => "light",
                    _ => throw new ArgumentException($"The argument {result} is not valid.")
                };
            }
        }
    }
}
