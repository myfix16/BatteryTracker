using BatteryTracker.Contracts.Services;
using BatteryTracker.Helpers;
using Microsoft.Windows.AppLifecycle;

namespace BatteryTracker.Activation;

// Triggered by redirection in Program.cs
public class AppActivationHandler : ActivationHandler<AppActivationArguments>
{
    private readonly IAppNotificationService _notificationService;

    public AppActivationHandler(IAppNotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    protected override bool CanHandleInternal(AppActivationArguments args)
    {
        // The args is passes by Program.Main() calling AppInstance.GetCurrent().GetActivatedEventArgs()
        return args.Kind == ExtendedActivationKind.Launch;
    }

    protected override async Task HandleInternalAsync(AppActivationArguments args)
    {
        // Prompt user that the app is already running
        _notificationService.Show("InstanceRunningMessage".Localized());

        await Task.CompletedTask;
    }
}
