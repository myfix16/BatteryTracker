using System.Collections.Generic;
using System.Linq;
using BatteryTracker.Activation;
using BatteryTracker.Contracts.Services;
using BatteryTracker.Views;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;

namespace BatteryTracker.Services;

public sealed class ActivationService : IActivationService
{
    private readonly ActivationHandler<AppActivationArguments> _defaultHandler;
    private readonly IEnumerable<IActivationHandler> _activationHandlers;
    private readonly IThemeSelectorService _themeSelectorService;
    private UIElement? _shell;

    public ActivationService(ActivationHandler<AppActivationArguments> defaultHandler, IEnumerable<IActivationHandler> activationHandlers, IThemeSelectorService themeSelectorService)
    {
        _defaultHandler = defaultHandler;
        _activationHandlers = activationHandlers;
        _themeSelectorService = themeSelectorService;
    }

    public async Task ActivateAsync(object activationArgs)
    {
        // Execute tasks before activation.
        await InitializeAsync();

        // Set the MainWindow Content.
        if (App.MainWindow.Content == null)
        {
            _shell = App.GetService<ShellPage>();
            App.MainWindow.Content = _shell ?? new Frame();
        }

        // Handle activation via ActivationHandlers.
        await HandleActivationAsync(activationArgs);

        // Execute tasks after activation.
        await StartupAsync();
    }

    private async Task HandleActivationAsync(object activationArgs)
    {
        IActivationHandler? activationHandler = _activationHandlers.FirstOrDefault(h => h.CanHandle(activationArgs));

        if (activationHandler != null)
        {
            await activationHandler.HandleAsync(activationArgs);
        }
        else if (_defaultHandler.CanHandle(activationArgs))
        {
            await _defaultHandler.HandleAsync(activationArgs);
        }
    }

    private async Task InitializeAsync()
    {
        await _themeSelectorService.InitializeAsync().ConfigureAwait(false);
        await Task.CompletedTask;
    }

    private async Task StartupAsync()
    {
        await _themeSelectorService.SetRequestedThemeAsync();
        await Task.CompletedTask;
    }
}
