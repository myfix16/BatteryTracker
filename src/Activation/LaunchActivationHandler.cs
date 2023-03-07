using System.Linq;
using BatteryTracker.Contracts.Services;
using BatteryTracker.ViewModels;
using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Activation;
using WinUIEx;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;

namespace BatteryTracker.Activation;

public class LaunchActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
{
    public const string OpenSettingsCommandArg = "--open-settings";

    public LaunchActivationHandler()
    {
    }

    protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
    {
        return AppInstance.GetCurrent().GetActivatedEventArgs()?.Kind == ExtendedActivationKind.Launch;
    }

    protected override async Task HandleInternalAsync(LaunchActivatedEventArgs args)
    {
        AppActivationArguments activationArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
        if (activationArgs.Data is ILaunchActivatedEventArgs launchArgs)
        {
            // Handle command line args
            string[] argStrings = launchArgs.Arguments.Split(' ');
            if (argStrings.Contains(OpenSettingsCommandArg))
            {
                App.GetService<INavigationService>().NavigateTo(typeof(SettingsViewModel).FullName!);
                App.MainWindow.Show();
            }
        }

        await Task.CompletedTask;
    }
}
