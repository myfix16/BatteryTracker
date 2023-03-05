using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatteryTracker.Helpers
{
    public static class StartProcessHelper
    {
        public const string ColorsSettings = "ms-settings:colors";

        public static void Start(string process)
        {
            Process.Start(new ProcessStartInfo(process) { UseShellExecute = true });
        }
    }
}
