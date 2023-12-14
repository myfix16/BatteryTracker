using BatteryTracker.Models;

namespace BatteryTracker.Contracts.Services;

public interface IPowerService
{
    PowerMode GetPowerMode();

    void SetPowerMode(PowerMode powerMode);
}