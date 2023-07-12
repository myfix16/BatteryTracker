using BatteryTracker.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace BatteryTracker.Views;

public sealed partial class BatteryInfoPage : Page
{
    public BatteryInfoViewModel ViewModel { get; }

    public BatteryInfoPage()
    {
        ViewModel = App.GetService<BatteryInfoViewModel>();
        InitializeComponent();
    }
}
