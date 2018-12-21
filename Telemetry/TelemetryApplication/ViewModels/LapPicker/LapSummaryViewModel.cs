namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.LapPicker
{
    using System;
    using SecondMonitor.ViewModels;
    using TelemetryManagement.DTO;

    public class LapSummaryViewModel : AbstractViewModel<LapSummaryDto>, ILapSummaryViewModel
    {

        private TimeSpan _lapTime;
        private int _lapNumber;

        public TimeSpan LapTime
        {
            get => _lapTime;
            set
            {
                _lapTime = value;
                NotifyPropertyChanged();
            }
        }

        public int LapNumber
        {
            get => _lapNumber;
            set
            {
                _lapNumber = value;
                NotifyPropertyChanged();
            }
        }

        public override void FromModel(LapSummaryDto model)
        {
            LapTime = model.LapTime;
            LapNumber = model.LapNumber;
        }

        public override LapSummaryDto SaveToNewModel()
        {
            return new LapSummaryDto()
            {
                LapNumber = LapNumber,
                LapTime = LapTime
            };
        }
    }
}