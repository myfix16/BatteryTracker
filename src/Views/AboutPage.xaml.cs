// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using BatteryTracker.ViewModels;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

// TODO: Set the URL for your privacy policy by updating SettingsPage_PrivacyTermsLink.NavigateUri in Resources.resw.
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
    }
}
