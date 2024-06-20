using System.Collections.Specialized;
using BatteryTracker.Contracts.Services;

namespace BatteryTracker.Tests.UnitTests.Mocks
{
    internal class MockAppNotificationService : IAppNotificationService
    {
        public void Initialize()
        {
        }

        public bool Show(string payload) => true;

        public NameValueCollection ParseArguments(string arguments) => [];

        public void Unregister()
        {
        }
    }
}
