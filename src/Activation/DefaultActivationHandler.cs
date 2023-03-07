using BatteryTracker.Contracts.Services;
using Microsoft.Windows.AppLifecycle;

namespace BatteryTracker.Activation;

public class DefaultActivationHandler : ActivationHandler<AppActivationArguments>
{
    private readonly INavigationService _navigationService;

    public DefaultActivationHandler(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    protected override bool CanHandleInternal(AppActivationArguments args)
    {
        // None of the ActivationHandlers has handled the activation.
        return _navigationService.Frame?.Content == null;
    }

    protected override async Task HandleInternalAsync(AppActivationArguments args)
    {
        await Task.CompletedTask;
    }
}
