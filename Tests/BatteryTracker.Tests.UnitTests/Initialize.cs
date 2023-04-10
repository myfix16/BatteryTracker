using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Windows.ApplicationModel.DynamicDependency;

[assembly: WinUITestTarget(typeof(BatteryTracker.App))]

namespace BatteryTracker.Tests.UnitTests;

[TestClass]
public class Initialize
{
    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext context)
    {
        // Initialize the appropriate version of the Windows App SDK.
        // This is required when testing MSIX apps that are framework-dependent on the Windows App SDK.
        // todo: bug - bootstrap failed
        bool success = Bootstrap.TryInitialize(0x00010002, out var hresult);
        if (!success)
        {
            Exception? exception = Marshal.GetExceptionForHR(hresult);
            Debug.WriteLine(exception?.Message);
        }
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        Bootstrap.Shutdown();
    }
}
