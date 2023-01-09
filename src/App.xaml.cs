// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
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
        static BatteryIcon? _batteryIcon;

        #region Constructors

        public App()
        {
            InitializeComponent();
        }

        #endregion

        #region Event Handlers

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            InitializeTrayIcon();
        }

        private void InitializeTrayIcon()
        {
            var exitApplicationCommand = (XamlUICommand)Resources["ExitApplicationCommand"];
            exitApplicationCommand.ExecuteRequested += ExitApplicationCommand_ExecuteRequested;

            var pushNotificationCommand = (XamlUICommand)Resources["SwitchNotificationCommand"];
            pushNotificationCommand.ExecuteRequested += (_, o) => _batteryIcon?.PushNotification("", "");

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
