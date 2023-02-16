using System.Runtime.InteropServices;

namespace BatteryTracker.Helpers
{
    internal class InformationHelper
    {
        [DllImport("ntdll.dll", SetLastError = true)]
        internal static extern uint RtlGetVersion(out OsVersionInfo versionInformation); // return type should be the NtStatus enum
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct OsVersionInfo
    {
        private readonly uint OsVersionInfoSize;

        public readonly uint MajorVersion;
        public readonly uint MinorVersion;

        public readonly uint BuildNumber;

        public readonly uint PlatformId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public readonly string CSDVersion;
    }
}
