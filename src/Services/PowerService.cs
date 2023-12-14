using BatteryTracker.Contracts.Services;
using BatteryTracker.Helpers;
using BatteryTracker.Models;
using Microsoft.Extensions.Logging;

namespace BatteryTracker.Services;

public sealed class PowerService : IPowerService
{
    readonly ILogger<PowerService> _logger;

    public PowerService(ILogger<PowerService> logger)
    {
        _logger = logger;
    }

    public PowerMode GetPowerMode()
    {
        uint result = PowerHelper.PowerGetActualOverlayScheme(out Guid powerModeId);
        if (result != 0)
        {
            _logger.LogError("Failed to fetch current power mode");
            throw new RuntimeException("Failed to fetch current power mode");
        }
        return PowerMode.GuidToPowerMode(powerModeId);
    }

    public void SetPowerMode(PowerMode powerMode)
    {
        uint result = PowerHelper.PowerSetActiveOverlayScheme(powerMode.Guid);
        if (result != 0)
        {
            _logger.LogError("Failed to set current power mode: {powerModeId}", powerMode);
            throw new RuntimeException($"Failed to set current power mode: {powerMode}");
        }
    }
}
