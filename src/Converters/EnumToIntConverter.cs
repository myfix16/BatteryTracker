using Microsoft.UI.Xaml.Data;

namespace BatteryTracker.Converters;

public class EnumToIntConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return (int)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (!Enum.IsDefined(typeof(ElementTheme), value))
        {
            throw new ArgumentException($"EnumToIndexConverter value out of range: {value}");
        }

        return (ElementTheme)value;
    }
}
