using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatteryTracker.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Windows.System.Power;

namespace BatteryTracker.ViewModels
{
    public sealed class BatteryInfoViewModel : ObservableRecipient
    {   
        private BatteryInfo _batteryInfo;

        public int ChargePercent
        {
            get => _batteryInfo.ChargePercent;
            set => SetProperty(ref _batteryInfo.ChargePercent, value);
        }

        public BatteryStatus BatteryStatus
        {
            get => _batteryInfo.BatteryStatus;
            set => SetProperty(ref _batteryInfo.BatteryStatus, value);
        }

        public PowerSupplyStatus PowerSupplyStatus
        {
            get => _batteryInfo.PowerSupplyStatus;
            set => SetProperty(ref _batteryInfo.PowerSupplyStatus, value);
        }

        public void UpdateStatus()
        {
            ChargePercent = PowerManager.RemainingChargePercent;
            BatteryStatus = PowerManager.BatteryStatus;
            PowerSupplyStatus = PowerManager.PowerSupplyStatus;
        }
    }
}
