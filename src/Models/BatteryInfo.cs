using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Windows.System.Power;

namespace BatteryTracker.Models;

public struct BatteryInfo
{
    public int ChargePercent;
    public BatteryStatus BatteryStatus;
    public PowerSupplyStatus PowerSupplyStatus;
}
