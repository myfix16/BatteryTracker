using Microsoft.Windows.System.Power;

namespace BatteryTracker.Models;

public struct BatteryInfo
{
    public int ChargePercent;
    public BatteryStatus BatteryStatus;
    public PowerSupplyStatus PowerSupplyStatus;
    public int DesignedCapacity;
    public int MaxCapacity;
    public int RemainingCapacity;
    public int ChargingRate;
}
