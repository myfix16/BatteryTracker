using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatteryTracker.Contracts.Services;

namespace BatteryTracker.Tests.UnitTests.Mocks
{
    internal class MockSettingsStorageService: ISettingsStorageService
    {
        public IDictionary<string, object> GetSettingsStorage()
        {
            return new Dictionary<string, object>();
        }
    }
}
