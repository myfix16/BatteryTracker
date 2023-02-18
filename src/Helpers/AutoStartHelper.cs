using Windows.ApplicationModel;
using Windows.UI.Popups;

namespace BatteryTracker.Helpers
{
    internal static class AutoStartHelper
    {
        private const string StartupTaskName = "BatteryTrackerStartup";

        private static StartupTask? _startupTask;

        internal static async Task<bool> IsRunAtStartup()
        {
            _startupTask ??= await StartupTask.GetAsync(StartupTaskName);
            StartupTaskState startupTaskState = _startupTask.State;
            return startupTaskState is StartupTaskState.Enabled or StartupTaskState.EnabledByPolicy;
        }

        internal static async Task DisableStartup()
        {
            _startupTask ??= await StartupTask.GetAsync(StartupTaskName);
            switch (_startupTask.State)
            {
                case StartupTaskState.Enabled:
                    _startupTask.Disable();
                    break;
                case StartupTaskState.EnabledByPolicy:
                    var dialog = new MessageDialog("Startup enabled by group policy, or not supported on this device");
                    await dialog.ShowAsync();
                    break;
            }
        }

        internal static async Task EnableStartup()
        {
            _startupTask ??= await StartupTask.GetAsync(StartupTaskName);
            switch (_startupTask.State)
            {
                // Task is disabled but can be enabled.
                case StartupTaskState.Disabled:
                    await _startupTask.RequestEnableAsync();
                    break;
                case StartupTaskState.DisabledByPolicy:
                    var dialog = new MessageDialog("Startup disabled by group policy, or not supported on this device");
                    await dialog.ShowAsync();
                    break;
            }
        }
    }
}
