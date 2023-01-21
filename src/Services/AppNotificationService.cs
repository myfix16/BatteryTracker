using System.Collections.Specialized;
using System.Web;
using BatteryTracker.Contracts.Services;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

namespace BatteryTracker.Services;

public class AppNotificationService : IAppNotificationService
{
    ~AppNotificationService()
    {
        Unregister();
    }

    public void Initialize()
    {
        // AppNotificationManager.Default.NotificationInvoked += OnNotificationInvoked;

        AppNotificationManager.Default.Register();
    }

    /// <summary>
    /// Handle notification invocations when the app is already running.
    /// </summary>
    public void OnNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
    {
        // App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        // {
        //     App.MainWindow.ShowMessageDialogAsync("Handle notification invocations when your app is already running.", "Notification Invoked");
        //
        //     App.MainWindow.BringToFront();
        // });

        // do nothing
    }

    public bool Show(string payload)
    {
        AppNotificationBuilder builder = new AppNotificationBuilder()
            .AddText(payload);

        AppNotification appNotification = builder.BuildNotification();
        AppNotificationManager.Default.Show(appNotification);

        return appNotification.Id != 0;
    }

    public NameValueCollection ParseArguments(string arguments) => HttpUtility.ParseQueryString(arguments);

    public void Unregister() => AppNotificationManager.Default.Unregister();
}
