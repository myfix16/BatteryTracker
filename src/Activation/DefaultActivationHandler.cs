﻿using BatteryTracker.Contracts.Services;

namespace BatteryTracker.Activation;

public class DefaultActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
{
    private readonly INavigationService _navigationService;

    public DefaultActivationHandler(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
    {
        // None of the ActivationHandlers has handled the activation.
        return _navigationService.Frame?.Content == null;
    }

    protected override async Task HandleInternalAsync(LaunchActivatedEventArgs args)
    {
        await Task.CompletedTask;
    }
}
