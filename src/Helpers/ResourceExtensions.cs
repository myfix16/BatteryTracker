using Microsoft.Windows.ApplicationModel.Resources;

namespace BatteryTracker.Helpers;

public static class ResourceExtensions
{
    public static readonly ResourceLoader ResourceLoader = new();

    public static string Localized(this string resourceKey) => ResourceLoader.GetString(resourceKey);
}
