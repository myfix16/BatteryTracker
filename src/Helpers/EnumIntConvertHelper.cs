using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatteryTracker.Helpers
{
    public class EnumIntConvertHelper
    {
        // todo: consider replacing converters with function binding
        public static int EnumToInt(object value) => (int)value;
    }
}
