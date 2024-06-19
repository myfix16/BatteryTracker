/*
 * Attribute should be given to this StackOverflow post:
 * https://stackoverflow.com/questions/61869347/control-windows-10s-power-mode-programmatically
 */

using System.Runtime.InteropServices;

namespace BatteryTracker.Helpers
{
    internal static class PowerHelper
    {
        [DllImport("powrprof.dll", EntryPoint = "PowerSetActiveOverlayScheme")]
        internal static extern uint PowerSetActiveOverlayScheme(Guid OverlaySchemeGuid);

        [DllImport("powrprof.dll", EntryPoint = "PowerGetActualOverlayScheme")]
        internal static extern uint PowerGetActualOverlayScheme(out Guid ActualOverlayGuid);

        [DllImport("powrprof.dll", EntryPoint = "PowerGetEffectiveOverlayScheme")]
        internal static extern uint PowerGetEffectiveOverlayScheme(out Guid EffectiveOverlayGuid);
    }
}
