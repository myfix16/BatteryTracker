using System.Diagnostics;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using Windows.System;

namespace BatteryTracker.Activation;

public class AppNotificationActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
{
    // private readonly IAppNotificationService _notificationService;

    public AppNotificationActivationHandler()
    {
        // _notificationService = notificationService;
    }

    protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
    {
        return AppInstance.GetCurrent().GetActivatedEventArgs()?.Kind == ExtendedActivationKind.AppNotification;
    }

    protected override async Task HandleInternalAsync(LaunchActivatedEventArgs args)
    {
        // Access the AppNotificationActivatedEventArgs.
        var activatedEventArgs = (AppNotificationActivatedEventArgs)AppInstance.GetCurrent().GetActivatedEventArgs().Data;

        // Handle the case when users click `Submit feedback` button on notifications
        if (activatedEventArgs.Arguments.TryGetValue("action", out string? value) && value == "SubmitFeedback")
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/myfix16/BatteryTracker/issues/new/choose"));
        }

        // quit
        Process.GetCurrentProcess().Kill();
    }
}
