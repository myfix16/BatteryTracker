using System.Diagnostics;
using BatteryTracker.Helpers;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;

namespace BatteryTracker.Activation;

public class AppNotificationActivationHandler : ActivationHandler<AppActivationArguments>
{
    // private readonly IAppNotificationService _notificationService;

    public AppNotificationActivationHandler()
    {
        // _notificationService = notificationService;
    }

    protected override bool CanHandleInternal(AppActivationArguments args)
    {
        return args.Kind == ExtendedActivationKind.AppNotification;
    }

    protected override async Task HandleInternalAsync(AppActivationArguments args)
    {
        // Access the AppNotificationActivatedEventArgs.
        var activatedEventArgs = (AppNotificationActivatedEventArgs)args.Data;

        // Handle the case when users click `Submit feedback` button on notifications
        if (activatedEventArgs.Arguments.TryGetValue("action", out string? value) && value == "SubmitFeedback")
        {
            await LaunchHelper.LaunchUriAsync(LaunchHelper.GitHubNewIssueUri);
            // Quit
            Process.GetCurrentProcess().Kill();
        }
    }
}
