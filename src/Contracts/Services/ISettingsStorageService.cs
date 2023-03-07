using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatteryTracker.Contracts.Services
{
    public interface ISettingsStorageService
    {
        IDictionary<string, object> GetSettingsStorage();
    }
}
