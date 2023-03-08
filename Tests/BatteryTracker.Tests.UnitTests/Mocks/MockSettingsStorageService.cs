using BatteryTracker.Contracts.Services;

namespace BatteryTracker.Tests.UnitTests.Mocks
{
    internal class MockSettingsStorageService : ISettingsStorageService
    {
        public IDictionary<string, object> GetSettingsStorage()
        {
            return new Dictionary<string, object>();
        }
    }
}
