using BatteryTracker.Models;
using Microsoft.UI.Xaml.Data;

namespace BatteryTracker.Converters;

public class PowerModeToIndexConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => (double)((PowerMode)value).Mode;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => (double)value switch
        {
            0 => PowerMode.BetterBattery,
            1 => PowerMode.Balanced,
            2 => PowerMode.BestPerformance,
            _ => PowerMode.Balanced
        };
}
