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
        public BatteryInfoWindow()
        {
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
                    BatteryInfoPage.ViewModel.UpdateStatus();
                    BringToFront();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(args), "Invalid WindowActivationState");
            }
        }
    }
}
