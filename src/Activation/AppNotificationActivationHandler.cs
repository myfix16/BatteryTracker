using System.Diagnostics;
using System.Runtime.Serialization;
using BatteryTracker.Contracts.Services;
using BatteryTracker.ViewModels;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;

namespace BatteryTracker.Activation;

public class AppNotificationActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
{
    protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
    {
        return AppInstance.GetCurrent().GetActivatedEventArgs()?.Kind == ExtendedActivationKind.AppNotification;
    }

    protected override async Task HandleInternalAsync(LaunchActivatedEventArgs args)
    {
        // Access the AppNotificationActivatedEventArgs.
        // var activatedEventArgs = (AppNotificationActivatedEventArgs)AppInstance.GetCurrent().GetActivatedEventArgs().Data;

        // Navigate to a specific page based on the notification arguments.
        // if (_notificationService.ParseArguments(activatedEventArgs.Argument)["action"] == "Settings")
        // {
        //     // Queue navigation with low priority to allow the UI to initialize.
        //     App.MainWindow.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () =>
        //     {
        //         _navigationService.NavigateTo(typeof(SettingsViewModel).FullName!);
        //     });
        // }

        // App.MainWindow.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () =>
        // {
        //     App.MainWindow.ShowMessageDialogAsync("Handle notification activations.", "Notification Activation");
        // });

        // ignore notification activation and quit
        Process.GetCurrentProcess().Kill();
        await Task.CompletedTask;
    }
}
