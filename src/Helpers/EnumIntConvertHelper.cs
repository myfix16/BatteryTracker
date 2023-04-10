namespace BatteryTracker.Helpers
{
    public static class EnumIntConvertHelper
    {
        // todo: consider replacing converters with function binding
        public static int EnumToInt(object value) => (int)value;
    }
}
