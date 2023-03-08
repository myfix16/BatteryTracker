using System.Collections.Specialized;
using BatteryTracker.Contracts.Services;

namespace BatteryTracker.Tests.UnitTests.Mocks
{
    internal class MockAppNotificationService : IAppNotificationService
    {
        public void Initialize()
        {
        }

        public bool Show(string payload)
        {
            return true;
        }

        public NameValueCollection ParseArguments(string arguments)
        {
            return new NameValueCollection();
        }

        public void Unregister()
        {
        }
    }
}
