using System.Diagnostics;
using System.Text;
using BatteryTracker.Activation;
using BatteryTracker.Contracts.Services;
using BatteryTracker.Helpers;
using BatteryTracker.Services;
using BatteryTracker.ViewModels;
using BatteryTracker.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Input;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Windows.Storage;
using WinUIEx;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;

namespace BatteryTracker;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public sealed partial class App : Application
{
    public static MainWindow MainWindow { get; } = new();

    public const string SettingsVersion = "v2";

    public bool HasLaunched { get; internal set; }

    private BatteryIcon? _batteryIcon;
    private readonly ILogger<App> _logger;
    private readonly INavigationService _navigationService;
    // private readonly IAppNotificationService _notificationService;

    private double _rastScale;

    #region Services

    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    private IHost Host { get; }

    public static T GetService<T>() where T : class
    {
        if ((Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }
        return service;
    }

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder().UseContentRoot(AppContext.BaseDirectory)
            .ConfigureServices((context, services) =>
            {
                // Default Activation Handler
                services.AddTransient<ActivationHandler<AppActivationArguments>, DefaultActivationHandler>();

                // Other Activation Handlers
                services.AddTransient<IActivationHandler, LaunchActivationHandler>();
                services.AddTransient<IActivationHandler, AppNotificationActivationHandler>();

                // Services
                services.AddSingleton<IAppNotificationService, AppNotificationService>();
                services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
                services.AddSingleton<IActivationService, ActivationService>();
                services.AddSingleton<IPageService, PageService>();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<ISettingsStorageService, AppLocalSettingsStorageService>();
                services.AddSingleton<ISettingsService, SettingsService>();
                services.AddTransient<INavigationViewService, NavigationViewService>();

                // Views and ViewModels
                services.AddTransient<SettingsViewModel>();
                services.AddTransient<SettingsPage>();
                services.AddTransient<ShellPage>();
                services.AddTransient<ShellViewModel>();
                services.AddTransient<AboutPage>();
                services.AddTransient<AboutViewModel>();

                // Taskbar icon
                services.AddSingleton<BatteryIcon>();

                // configure Serilog
                services.AddLogging();
            }).Build();

        GetService<IAppNotificationService>().Initialize();

        // Tell the logging service to use Serilog.File extension.
        string fullPath = $"{ApplicationData.Current.LocalFolder.Path}\\Logs\\App.log";
        GetService<ILoggerFactory>().AddFile(fullPath);
        _logger = GetService<ILogger<App>>();

        _navigationService = GetService<INavigationService>();
        // _notificationService = GetService<IAppNotificationService>();

        UnhandledException += App_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_OnUnobservedTaskException;
    }

    #endregion

    #region Event Handlers

    public async void OnXamlRootChanged(XamlRoot sender, XamlRootChangedEventArgs _)
    {
        // Check whether DPI has changed
        if (Math.Abs(_rastScale - sender.RasterizationScale) < 0.0000001 && _batteryIcon != null)
        {
            _rastScale = sender.RasterizationScale;
            await _batteryIcon.AdaptToDpiChange(sender.RasterizationScale);
        }
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        AppActivationArguments activationArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
        _logger.LogInformation($"App launched with activation kind: {activationArgs.Kind}");

        await GetService<IActivationService>().ActivateAsync(activationArgs);

        MainWindow.AppWindow.Closing += AppWindow_Closing;

        await InitializeTrayIconAsync();
    }

    private static void AppWindow_Closing(AppWindow _, AppWindowClosingEventArgs args)
    {
        // Closing the window will terminate the application, so hide it instead
        MainWindow.Hide();
        args.Cancel = true;
    }

    private void OpenSettingsCommand_ExecuteRequested(object? _, ExecuteRequestedEventArgs args)
    {
        if (MainWindow.Visible)
        {
            MainWindow.BringToFront();
            return;
        }

        MainWindow.Show();

        _navigationService.NavigateTo(typeof(SettingsViewModel).FullName!);
    }

    private void ExitApplicationCommand_ExecuteRequested(object? _, ExecuteRequestedEventArgs args)
    {
        _logger.LogInformation("User quits the app");
        _batteryIcon?.Dispose();
        Exit();
    }

    private void App_UnhandledException(object? _, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
        ProcessUnhandledException(e.Exception, true);
    }

    private void TaskScheduler_OnUnobservedTaskException(object? _, UnobservedTaskExceptionEventArgs e)
    {
        ProcessUnhandledException(e.Exception, false);
    }

    #endregion

    public async Task AdaptToDpiChangeAsync(double rastScale)
    {
        if (_batteryIcon != null)
        {
            await _batteryIcon.AdaptToDpiChange(rastScale);
        }
    }

    private async Task InitializeTrayIconAsync()
    {
        var openSettingsCommand = (XamlUICommand)Resources["OpenSettingsCommand"];
        openSettingsCommand.ExecuteRequested += OpenSettingsCommand_ExecuteRequested;

        var exitApplicationCommand = (XamlUICommand)Resources["ExitApplicationCommand"];
        exitApplicationCommand.ExecuteRequested += ExitApplicationCommand_ExecuteRequested;

        _batteryIcon = GetService<BatteryIcon>();
        await _batteryIcon.InitAsync(MainWindow.BatteryTrayIcon);
    }

    private void ProcessUnhandledException(Exception? e, bool showNotification)
    {
        _logger.LogCritical(e, "Unhandled exception");

        StringBuilder formattedException = new() { Capacity = 200 };
        formattedException.Append("--------- UNHANDLED EXCEPTION ---------");
        if (e is not null)
        {
            formattedException.Append($"\n>>>> HRESULT: {e.HResult}\n");

            formattedException.Append("\n--- MESSAGE ---");
            formattedException.Append(e.Message);
            if (e.StackTrace is not null)
            {
                formattedException.Append("\n--- STACKTRACE ---");
                formattedException.Append(e.StackTrace);
            }
            if (e.Source is not null)
            {
                formattedException.Append("\n--- SOURCE ---");
                formattedException.Append(e.Source);
            }
            if (e.InnerException is not null)
            {
                formattedException.Append("\n--- INNER ---");
                formattedException.Append(e.InnerException);
            }
        }
        else
        {
            formattedException.Append("\nException is null!\n");
        }

        formattedException.Append("---------------------------------------");

        Debug.WriteLine(formattedException.ToString());

        Debugger.Break(); // Please check "Output Window" for exception details (View -> Output Window) (CTRL + ALT + O)

        if (showNotification)
        {
            AppNotificationBuilder notificationBuilder = new AppNotificationBuilder()
                .AddText("UnhandledExceptionMessage".Localized())
                .AddButton(new AppNotificationButton("SubmitFeedback".Localized())
                    .AddArgument("action", "SubmitFeedback"));
            AppNotificationManager.Default.Show(notificationBuilder.BuildNotification());
        }

        Process.GetCurrentProcess().Kill();
    }
}