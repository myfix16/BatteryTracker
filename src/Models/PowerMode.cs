/*
 * Attribute should be given to FluentFlyouts3:
 * https://github.com/FireCubeStudios/FluentFlyouts3
 */

namespace BatteryTracker.Models;

public enum PowerModeEnum
{
    BetterBattery = 0,
    Balanced = 1,
    BestPerformance = 2
}

public static class PowerModeIds
{
    public static readonly Guid BetterBattery = new("961cc777-2547-4f9d-8174-7d86181b8a7a");
    public static readonly Guid Balanced = new("00000000-0000-0000-0000-000000000000");
    public static readonly Guid BestPerformance = new("ded574b5-45a0-4f42-8737-46345c09c238");
}

public sealed record PowerMode(PowerModeEnum Mode, Guid Guid)
{
    public static readonly PowerMode BetterBattery =
        new(PowerModeEnum.BetterBattery, PowerModeIds.BetterBattery);

    public static readonly PowerMode Balanced =
        new(PowerModeEnum.Balanced, PowerModeIds.Balanced);

    public static readonly PowerMode BestPerformance =
        new(PowerModeEnum.BestPerformance, PowerModeIds.BestPerformance);

    public static PowerMode GuidToPowerMode(Guid guid)
    {
        if (guid == PowerModeIds.BetterBattery)
        {
            return BetterBattery;
        }
        if (guid == PowerModeIds.Balanced)
        {
            return Balanced;
        }
        if (guid == PowerModeIds.BestPerformance)
        {
            return BestPerformance;
        }

        throw new ArgumentOutOfRangeException($"Power mode guid {guid} is invalid!");
    }

    public override string ToString()
    {
        return $"{Mode} {Guid}";
    }
}
