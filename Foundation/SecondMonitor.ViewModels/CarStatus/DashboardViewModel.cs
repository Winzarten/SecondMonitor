namespace SecondMonitor.ViewModels.CarStatus
{
    using System.Diagnostics;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;
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
        }

        public StatusIconViewModel EngineStatus{ get; }
        public StatusIconViewModel TransmissionStatus { get; }
        public StatusIconViewModel SuspensionStatus { get; }
        public StatusIconViewModel BodyworkStatus { get; }

        public void ApplyDateSet(SimulatorDataSet dataSet)
        {
            if (dataSet?.PlayerInfo?.CarInfo?.CarDamageInformation == null || _refreshStopwatch.ElapsedMilliseconds < 1000)
            {
                return;
            }

            CarDamageInformation carDamage = dataSet?.PlayerInfo?.CarInfo?.CarDamageInformation;

            ApplyDamage(carDamage.Bodywork, BodyworkStatus );
            ApplyDamage(carDamage.Engine, EngineStatus);
            ApplyDamage(carDamage.Suspension, SuspensionStatus);
            ApplyDamage(carDamage.Transmission, TransmissionStatus);

            _refreshStopwatch.Restart();
        }

        public void Reset()
        {

        }

        private static void ApplyDamage(DamageInformation damageInformation, StatusIconViewModel viewModel)
        {
            viewModel.AdditionalText = (damageInformation.Damage * 100).ToString("N3");

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