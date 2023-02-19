using System.Reflection;
using BatteryTracker.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.ApplicationModel;

namespace BatteryTracker.ViewModels
{
    public class AboutViewModel : ObservableRecipient
    {
        private string _versionDescription;

        public string VersionDescription
        {
            get => _versionDescription;
            set => SetProperty(ref _versionDescription, value);
        }

        public AboutViewModel()
        {
            _versionDescription = GetVersionDescription();
        }

        private static string GetVersionDescription()
        {
            Version version;

            if (RuntimeHelper.IsMSIX)
            {
                var packageVersion = Package.Current.Id.Version;

                version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
            }
            else
            {
                version = Assembly.GetExecutingAssembly().GetName().Version!;
            }

            return
                $"{"AppDisplayName".Localized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
