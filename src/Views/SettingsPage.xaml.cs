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
}
