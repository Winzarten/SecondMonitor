namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.LapPicker
{
    using System;
    using SecondMonitor.ViewModels;
    using TelemetryManagement.DTO;

    public class LapSummaryViewModel : AbstractViewModel<LapSummaryDto>, ILapSummaryViewModel
    {

        private TimeSpan _lapTime;
        private int _lapNumber;
        private bool _display;

        public TimeSpan LapTime
        {
            get => _lapTime;
            set
            {
                _lapTime = value;
                NotifyPropertyChanged();
            }
        }

        public bool Selected
        {
            get => _display;
            set
            {
                _display = value;
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

        protected override void ApplyModel(LapSummaryDto model)
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