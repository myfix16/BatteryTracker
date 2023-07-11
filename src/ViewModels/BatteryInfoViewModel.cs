using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BatteryTracker.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Windows.System.Power;
using Windows.Devices.Power;
using Microsoft.Extensions.Logging;

namespace BatteryTracker.ViewModels
{
    public sealed class BatteryInfoViewModel : ObservableRecipient
    {
        #region Private fields

        BatteryInfo _batteryInfo;

        readonly ILogger<BatteryInfoViewModel> _logger;

        #endregion

        #region Properties

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

        public int DesignedCapacity
        {
            get => _batteryInfo.DesignedCapacity;
            set => SetProperty(ref _batteryInfo.DesignedCapacity, value);
        }

        public int MaxCapacity
        {
            get => _batteryInfo.MaxCapacity;
            set => SetProperty(ref _batteryInfo.MaxCapacity, value);
        }

        public int RemainingCapacity
        {
            get => _batteryInfo.RemainingCapacity;
            set => SetProperty(ref _batteryInfo.RemainingCapacity, value);
        }

        public int ChargingRate
        {
            get => _batteryInfo.ChargingRate;
            set => SetProperty(ref _batteryInfo.ChargingRate, value);
        }

        #endregion

        public BatteryInfoViewModel(ILogger<BatteryInfoViewModel> logger)
        {
            _logger = logger;
        }

        public void UpdateStatus()
        {
            ChargePercent = PowerManager.RemainingChargePercent;
            BatteryStatus = PowerManager.BatteryStatus;
            PowerSupplyStatus = PowerManager.PowerSupplyStatus;

            BatteryReport info = Battery.AggregateBattery.GetReport();
            if (info == null)
            {
                _logger.LogError("Failed to fetch battery report");
            }
            else
            {
                DesignedCapacity = info.DesignCapacityInMilliwattHours!.Value;
                MaxCapacity = info.FullChargeCapacityInMilliwattHours!.Value;
                RemainingCapacity = info.RemainingCapacityInMilliwattHours!.Value;
                ChargingRate = info.ChargeRateInMilliwatts!.Value;
            }
        }

        #region Binding functions

        internal static string GetRemainingCapacityText(int capacity)
            => $"{(double)capacity / 1000:0.##} Wh";

        internal static string GetBatteryHealthText(int maxCapacity, int designedCapacity)
            => $"{(double)maxCapacity * 100 / designedCapacity:0.#}% " +
               $"({(double)maxCapacity / 1000:0.##} Wh / {(double)designedCapacity / 1000} Wh)";

        internal static string GetChargingRateText(int rate, BatteryStatus status) 
            => $"{(double)rate / 1000:0.##} Wh ({status})";

        #endregion
    }
}
