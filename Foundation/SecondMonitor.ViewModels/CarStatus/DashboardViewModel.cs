namespace SecondMonitor.ViewModels.CarStatus
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using DataModel.BasicProperties;
    using DataModel.Extensions;
    using DataModel.Snapshot;
    using DataModel.Snapshot.Drivers;
    using DataModel.Snapshot.Systems;
    using StatusIcon;

    public class DashboardViewModel : AbstractViewModel, ISimulatorDataSetViewModel
    {
        private readonly Stopwatch _refreshStopwatch;

        public DashboardViewModel()
        {
            _refreshStopwatch = Stopwatch.StartNew();
            EngineStatus = new StatusIconViewModel();
            TransmissionStatus = new StatusIconViewModel();
            SuspensionStatus = new StatusIconViewModel();
            BodyworkStatus = new StatusIconViewModel();

            PitLimiterStatus = new StatusIconViewModel();
            AlternatorStatus = new StatusIconViewModel();
            TyreDirtStatus = new StatusIconViewModel();
            DrsStatusIndication = new StatusIconViewModel();
            BoostIndication = new StatusIconViewModel();
        }

        public StatusIconViewModel EngineStatus{ get; }
        public StatusIconViewModel TransmissionStatus { get; }
        public StatusIconViewModel SuspensionStatus { get; }
        public StatusIconViewModel BodyworkStatus { get; }

        public StatusIconViewModel PitLimiterStatus { get; }
        public StatusIconViewModel AlternatorStatus { get;}
        public StatusIconViewModel TyreDirtStatus { get; }
        public StatusIconViewModel DrsStatusIndication { get; }
        public StatusIconViewModel BoostIndication { get; }

        public void ApplyDateSet(SimulatorDataSet dataSet)
        {
            if (dataSet?.PlayerInfo?.CarInfo?.CarDamageInformation == null || _refreshStopwatch.ElapsedMilliseconds < 200)
            {
                return;
            }

            CarDamageInformation carDamage = dataSet?.PlayerInfo?.CarInfo?.CarDamageInformation;

            ApplyDamage(carDamage.Bodywork, BodyworkStatus );
            ApplyDamage(carDamage.Engine, EngineStatus);
            ApplyDamage(carDamage.Suspension, SuspensionStatus);
            ApplyDamage(carDamage.Transmission, TransmissionStatus);
            UpdatePitLimiterStatus(dataSet.PlayerInfo);
            UpdateAlternatorStatus(dataSet.PlayerInfo.CarInfo);
            UpdateDirtLevel(dataSet.PlayerInfo.CarInfo);
            UpdateDrsStatus(dataSet.PlayerInfo.CarInfo);
            UpdateBoostStatus(dataSet.PlayerInfo.CarInfo.BoostSystem);

            if (dataSet.PlayerInfo.CarInfo.WheelsInfo.AllWheels.Any(x => x.Detached))
            {
                SuspensionStatus.IconState = StatusIconState.Error;
            }

            _refreshStopwatch.Restart();
        }

        private void UpdateBoostStatus(BoostSystem boostSystem)
        {
            if (boostSystem.CooldownTimer == TimeSpan.Zero && boostSystem.TimeRemaining == TimeSpan.Zero)
            {
                BoostIndication.AdditionalText = boostSystem.ActivationsRemaining < 0 ? string.Empty : boostSystem.ActivationsRemaining.ToString();
            }
            else
            {
                BoostIndication.AdditionalText = boostSystem.TimeRemaining == TimeSpan.Zero ? boostSystem.CooldownTimer.TotalSeconds.ToString("F0") : boostSystem.TimeRemaining.TotalSeconds.ToString("F0");
            }

            switch (boostSystem.BoostStatus)
            {
                case BoostStatus.UnAvailable:
                    BoostIndication.IconState = StatusIconState.Unlit;
                    break;
                case BoostStatus.Available:
                    BoostIndication.IconState = StatusIconState.Information;
                    break;
                case BoostStatus.InUse:
                    BoostIndication.IconState = StatusIconState.Ok;
                    break;
                case BoostStatus.Cooldown:
                    BoostIndication.IconState = StatusIconState.Warning;
                    break;
            }
        }

        private void UpdateDrsStatus(CarInfo playerInfoCarInfo)
        {
            DrsStatusIndication.AdditionalText = playerInfoCarInfo.DrsSystem.DrsActivationLeft < 0 ? string.Empty : playerInfoCarInfo.DrsSystem.DrsActivationLeft.ToString();
            switch (playerInfoCarInfo.DrsSystem.DrsStatus)
            {
                case DrsStatus.Available:
                    DrsStatusIndication.IconState = StatusIconState.Information;
                    break;
                case DrsStatus.InUse:
                    DrsStatusIndication.IconState = StatusIconState.Ok;
                    break;
                default:
                    DrsStatusIndication.IconState = StatusIconState.Unlit;
                    break;
            }
        }

        private void UpdateDirtLevel(CarInfo playerCar)
        {
            double maxDirt = playerCar.WheelsInfo.AllWheels.Max(x => x.DirtLevel);
            if (maxDirt < 0.01)
            {
                TyreDirtStatus.IconState = StatusIconState.Unlit;
                TyreDirtStatus.AdditionalText = string.Empty;
                return;
            }

            TyreDirtStatus.AdditionalText = ((int)( maxDirt * 100)).ToString();
            TyreDirtStatus.IconState = maxDirt > 0.5 ? StatusIconState.Error : StatusIconState.Warning;
        }

        private void UpdateAlternatorStatus(CarInfo playerInfoCarInfo)
        {
            AlternatorStatus.IconState = playerInfoCarInfo.EngineRpm < 10 ? StatusIconState.Error : StatusIconState.Unlit;
        }

        public void Reset()
        {

        }

        private void UpdatePitLimiterStatus(DriverInfo driver)
        {
            if (driver.InPits)
            {
                PitLimiterStatus.IconState = driver.CarInfo.SpeedLimiterEngaged ? StatusIconState.Ok : StatusIconState.Warning;
                return;
            }

            PitLimiterStatus.IconState = driver.CarInfo.SpeedLimiterEngaged ? StatusIconState.Error : StatusIconState.Unlit;
        }

        private static void ApplyDamage(DamageInformation damageInformation, StatusIconViewModel viewModel)
        {
            viewModel.AdditionalText = damageInformation.Damage > 0 ? Math.Ceiling(damageInformation.Damage * 100).ToString("F0") : string.Empty;

            if (damageInformation.Damage < damageInformation.MediumDamageThreshold)
            {
                viewModel.IconState = StatusIconState.Unlit;
                return;
            }

            if (damageInformation.Damage < damageInformation.HeavyDamageThreshold)
            {
                viewModel.IconState = StatusIconState.Warning;
                return;
            }

            viewModel.IconState = StatusIconState.Error;
        }
    }
}