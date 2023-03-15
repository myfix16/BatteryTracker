using System.Reflection;
using System.Windows.Input;
using BatteryTracker.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Helpers;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.System;

namespace BatteryTracker.ViewModels
{
    public sealed class AboutViewModel : ObservableRecipient
    {
        private bool _showCopiedMessage;

        public string VersionDescription { get; }

        public bool ShowCopiedMessage
        {
            get => _showCopiedMessage;
            set => SetProperty(ref _showCopiedMessage, value);
        }

        public ICommand CopyAppVersionCommand { get; }

        public ICommand CopyWindowsVersionCommand { get; }

        public ICommand OpenLogFolderCommand { get; }

        public ICommand OpenGitHubRepoCommand { get; }

        public ICommand OpenPrivacyStatementCommand { get; }

        public AboutViewModel()
        {
            const int copiedMessageDisplayTime = 2000;

            VersionDescription = GetVersionDescription();

            CopyAppVersionCommand = new AsyncRelayCommand(async () =>
            {
                DataPackage package = new() { RequestedOperation = DataPackageOperation.Copy };
                package.SetText(VersionDescription[1..]);
                Clipboard.SetContent(package);

                // Show copied message
                ShowCopiedMessage = true;
                await Task.Delay(copiedMessageDisplayTime);
                ShowCopiedMessage = false;
            });

            CopyWindowsVersionCommand = new AsyncRelayCommand(async () =>
            {
                DataPackage package = new() { RequestedOperation = DataPackageOperation.Copy };
                package.SetText(SystemInformation.Instance.OperatingSystemVersion.ToString());
                Clipboard.SetContent(package);

                // Show copied message
                ShowCopiedMessage = true;
                await Task.Delay(copiedMessageDisplayTime);
                ShowCopiedMessage = false;
            });

            OpenLogFolderCommand = new AsyncRelayCommand(async () =>
            {
                await Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder);
            });

            OpenGitHubRepoCommand = new AsyncRelayCommand(async () =>
            {
                await LaunchHelper.LaunchUriAsync(LaunchHelper.GitHubRepoUri);
            });

            OpenPrivacyStatementCommand = new AsyncRelayCommand(async () =>
            {
                await LaunchHelper.LaunchUriAsync(LaunchHelper.PrivacyStatementUri);
            });
        }

        private static string GetVersionDescription()
        {
            Version version;

            if (RuntimeHelper.IsMSIX)
            {
                PackageVersion packageVersion = Package.Current.Id.Version;

                version = new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build,
                    packageVersion.Revision);
            }
            else
            {
                version = Assembly.GetExecutingAssembly().GetName().Version!;
            }

            return $"v{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
