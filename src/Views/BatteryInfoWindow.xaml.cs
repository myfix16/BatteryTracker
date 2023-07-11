using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using BatteryTracker.Models;
using BatteryTracker.ViewModels;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.System.Power;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BatteryTracker.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BatteryInfoWindow : WindowEx
    {
        public BatteryInfoViewModel ViewModel { get; }

        public BatteryInfoWindow()
        {
            ViewModel = new BatteryInfoViewModel();
            InitializeComponent();
            Activated += OnActivated;
            Closed += BatteryInfoWindow_Closed;
        }

        void BatteryInfoWindow_Closed(object sender, WindowEventArgs args)
        {
            args.Handled = true;
            this.Hide();
        }

        void OnActivated(object sender, WindowActivatedEventArgs args)
        {
            switch (args.WindowActivationState)
            {
                // Hide the window on losing focus
                case WindowActivationState.Deactivated:
                    // this.Hide();
                    break;
                // Fetch the latest battery info and display the window
                case WindowActivationState.CodeActivated:
                case WindowActivationState.PointerActivated:
                    ViewModel.UpdateStatus();
                    BringToFront();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
