namespace BatteryTracker.Models;

public sealed class RuntimeException : ApplicationException
{
    public RuntimeException(string message) : base(message) { }
}