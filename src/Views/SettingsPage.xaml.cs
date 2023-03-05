using BatteryTracker.Helpers;
using BatteryTracker.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace BatteryTracker.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel { get; }

    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();
    }

    private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
    {
        await LaunchHelper.LaunchUriAsync(LaunchHelper.ColorsSettingsUri);
    }
}
