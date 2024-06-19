using BatteryTracker.Contracts.Services;
using BatteryTracker.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Windows.System.Power;
using Windows.Devices.Power;
using BatteryTracker.Helpers;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;

namespace BatteryTracker.ViewModels
{
    public sealed class BatteryInfoViewModel : ObservableRecipient
    {
        #region Private fields

        readonly DispatcherQueue _dispatcher = DispatcherQueue.GetForCurrentThread();

        BatteryInfo _batteryInfo;
        PowerMode _powerMode;
        int _refreshInterval = 5000;  // todo: allow customizing refresh interval in settings

        readonly ILogger<BatteryInfoViewModel> _logger;
        readonly IPowerService _powerService;

        readonly Timer _timer;

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
            private set => SetProperty(ref _batteryInfo.BatteryStatus, value);
        }

        public PowerSupplyStatus PowerSupplyStatus
        {
            get => _batteryInfo.PowerSupplyStatus;
            set => SetProperty(ref _batteryInfo.PowerSupplyStatus, value);
        }

        public int DesignedCapacity
        {
            get => _batteryInfo.DesignedCapacity;
            private set => SetProperty(ref _batteryInfo.DesignedCapacity, value);
        }

        public int MaxCapacity
        {
            get => _batteryInfo.MaxCapacity;
            private set => SetProperty(ref _batteryInfo.MaxCapacity, value);
        }

        public int RemainingCapacity
        {
            get => _batteryInfo.RemainingCapacity;
            private set => SetProperty(ref _batteryInfo.RemainingCapacity, value);
        }

        public int ChargingRate
        {
            get => _batteryInfo.ChargingRate;
            private set => SetProperty(ref _batteryInfo.ChargingRate, value);
        }

        public PowerMode PowerMode
        {
            get => _powerMode;
            set
            {
                _powerService.SetPowerMode(value);
                SetProperty(ref _powerMode, value);
            }
        }

        #endregion

        public BatteryInfoViewModel(ILogger<BatteryInfoViewModel> logger, IPowerService powerService)
        {
            _logger = logger;
            _powerService = powerService;
            _powerMode = PowerMode.Balanced;
            PowerMode = _powerService.GetPowerMode();

            _timer = new Timer();
            _timer.Action += UpdateStatus;
        }

        ~BatteryInfoViewModel()
        {
            _timer.Action -= UpdateStatus;
        }

        public void StartUpdatingStatus()
        {
            UpdateStatus();
            _timer.StartTimer(_refreshInterval);
        }

        public void StopUpdatingStatus() => _timer.StopTimer();

        void UpdateStatus()
        {
            _dispatcher.TryEnqueue(() =>
            {
                // Update battery info
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
            });
        }

        #region Binding functions

        internal static string GetRemainingCapacityText(int capacity)
            => $"{(double)capacity / 1000:0.##} Wh";

        internal static string GetBatteryHealthText(int maxCapacity, int designedCapacity)
            => $"{(double)maxCapacity * 100 / designedCapacity:0.#}% " +
               $"({(double)maxCapacity / 1000:0.##} Wh / {(double)designedCapacity / 1000} Wh)";

        internal static string GetChargingRateText(int rate, BatteryStatus status)
            => $"{(double)rate / 1000:0.##} W ({status})";

        // todo: localize this text
        internal static string GetPowerModeText(PowerMode powerMode)
            => $"Power Mode: {powerMode}";

        #endregion
    }
}
