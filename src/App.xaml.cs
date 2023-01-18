// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using BatteryTracker.Activation;
using BatteryTracker.Contracts.Services;
using BatteryTracker.Helpers;
using BatteryTracker.Services;
using BatteryTracker.ViewModels;
using BatteryTracker.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Input;
using Microsoft.Windows.AppLifecycle;
using WinUIEx;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BatteryTracker
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private BatteryIcon? _batteryIcon;

        public static WindowEx MainWindow { get; } = new MainWindow();

        #region Services

        // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        public IHost Host { get; }

        public static T GetService<T>() where T : class
        {
            if ((Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
            {
                throw new ArgumentException(
                    $"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
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
                    services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

                    // Other Activation Handlers
                    // services.AddTransient<IActivationHandler, AppNotificationActivationHandler>();

                    // Services
                    services.AddSingleton<IAppNotificationService, AppNotificationService>();
                    services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
                    services.AddTransient<INavigationViewService, NavigationViewService>();

                    services.AddSingleton<IActivationService, ActivationService>();
                    services.AddSingleton<IPageService, PageService>();
                    services.AddSingleton<INavigationService, NavigationService>();

                    // Views and ViewModels
                    services.AddTransient<SettingsViewModel>();
                    services.AddTransient<SettingsPage>();
                    services.AddTransient<ShellPage>();
                    services.AddTransient<ShellViewModel>();
                    services.AddTransient<AboutPage>();
                    services.AddTransient<AboutViewModel>();

                    // Taskbar icon
                    services.AddTransient<BatteryIcon>();
                }).Build();

            // App.GetService<IAppNotificationService>().Initialize();

            UnhandledException += App_UnhandledException;
        }

        #endregion

        #region Event Handlers

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Only allow single instance to run
            // Get or register the main instance
            var mainInstance = AppInstance.FindOrRegisterForKey("main");

            // If the main instance isn't this current instance
            if (!mainInstance.IsCurrent)
            {
                // Prompt user that the app is already running and exit our instance
                NotificationManager.PushMessage("Another instance is already running.");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                return;
            }

            MainWindow.AppWindow.Closing += AppWindow_Closing;

            await GetService<IActivationService>().ActivateAsync(args);

            InitializeTrayIcon();
        }

        private static void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            // closing the window will terminate the application, so hide it instead
            args.Cancel = true;
            MainWindow.Hide();
        }

        private static void OpenSettingsCommand_ExecuteRequested(object? _, ExecuteRequestedEventArgs args)
        {
            // MainWindow.Content ??= GetService<ShellPage>();
            GetService<INavigationService>().NavigateTo(typeof(SettingsViewModel).FullName!);
            MainWindow.Show();
        }

        private void ExitApplicationCommand_ExecuteRequested(object? _, ExecuteRequestedEventArgs args)
        {
            _batteryIcon?.Dispose();
            Exit();
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // TODO: Log and handle exceptions as appropriate.
            // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
        }

        #endregion

        private void InitializeTrayIcon()
        {
            var openSettingsCommand = (XamlUICommand)Resources["OpenSettingsCommand"];
            openSettingsCommand.ExecuteRequested += OpenSettingsCommand_ExecuteRequested;

            var exitApplicationCommand = (XamlUICommand)Resources["ExitApplicationCommand"];
            exitApplicationCommand.ExecuteRequested += ExitApplicationCommand_ExecuteRequested;

            _batteryIcon = GetService<BatteryIcon>();
            _batteryIcon.Init(Resources);
        }
    }
}
