using BatteryTracker.Contracts.Services;
using Microsoft.Windows.AppLifecycle;

namespace BatteryTracker.Activation;

public sealed class DefaultActivationHandler(INavigationService navigationService)
    : ActivationHandler<AppActivationArguments>
{
    protected override bool CanHandleInternal(AppActivationArguments args)
    {
        // None of the ActivationHandlers has handled the activation.
        return navigationService.Frame?.Content == null;
    }

    protected override async Task HandleInternalAsync(AppActivationArguments args)
    {
        await Task.CompletedTask;
    }
}
