using BatteryTracker.Helpers;
using BatteryTracker.ViewModels;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BatteryTracker.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        public AboutViewModel ViewModel { get; }

        public AboutPage()
        {
            ViewModel = App.GetService<AboutViewModel>();
            InitializeComponent();
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            LaunchHelper.StartProcess("mailto:myfix16@outlook.com?subject=Battery%20Tracker%20Feedback");
        }
    }
}
