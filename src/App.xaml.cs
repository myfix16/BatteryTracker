// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.System.Power;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BatteryTracker
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private BatteryIcon? _batteryIcon;

        public App()
        {
            InitializeComponent();
        }

        #region Event Handlers

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Only allow single instance to run
            // Get or register the main instance
            var mainInstance = AppInstance.FindOrRegisterForKey("main");

            // If the main instance isn't this current instance
            if (!mainInstance.IsCurrent)
            {
                // Prompt user that the app is already running and exit our instance
                NotificationManager.PushMessage("Another instance is already running.");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                return;
            }

            InitializeTrayIcon();
        }

        private void InitializeTrayIcon()
        {
            var exitApplicationCommand = (XamlUICommand)Resources["ExitApplicationCommand"];
            exitApplicationCommand.ExecuteRequested += ExitApplicationCommand_ExecuteRequested;

            var pushNotificationCommand = (XamlUICommand)Resources["SwitchNotificationCommand"];
            pushNotificationCommand.ExecuteRequested += (_, o) =>
                NotificationManager.PushMessage($"Battery: {PowerManager.RemainingChargePercent}%");

            _batteryIcon = new BatteryIcon();
            _batteryIcon.Init(Resources);
        }

        private void ExitApplicationCommand_ExecuteRequested(object? _, ExecuteRequestedEventArgs args)
        {
            _batteryIcon?.Dispose();
        }

        #endregion
    }
}
