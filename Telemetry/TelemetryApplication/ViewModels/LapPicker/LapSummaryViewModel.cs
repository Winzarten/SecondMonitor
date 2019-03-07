namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.LapPicker
{
    using System;
    using System.Windows.Media;
    using Controllers.Synchronization;
    using SecondMonitor.ViewModels;
    using TelemetryManagement.DTO;

    public class LapSummaryViewModel : AbstractViewModel<LapSummaryDto>, ILapSummaryViewModel
    {

        private TimeSpan _lapTime;
        private string _lapNumber;
        private bool _display;
        private Color _lapColor;
        private TimeSpan _sector1Time;
        private TimeSpan _sector2Time;
        private TimeSpan _sector3Time;

        public ILapColorSynchronization LapColorSynchronization { get; set; }

        public TimeSpan LapTime
        {
            get => _lapTime;
            set
            {
                _lapTime = value;
                NotifyPropertyChanged();
            }
        }

        public TimeSpan Sector1Time
        {
            get => _sector1Time;
            set => SetProperty(ref _sector1Time, value);
        }

        public TimeSpan Sector2Time
        {
            get => _sector2Time;
            set => SetProperty(ref _sector2Time, value);
        }

        public TimeSpan Sector3Time
        {
            get => _sector3Time;
            set => SetProperty(ref _sector3Time, value);
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

        public Color LapColor
        {
            get => _lapColor;
            set
            {
                _lapColor = value;
                LapColorSynchronization?.SetColorForLap(OriginalModel.Id, value);
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(LapColorBrush));
            }
        }

        public SolidColorBrush LapColorBrush => new SolidColorBrush(LapColor);

        public string LapNumber
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
            LapNumber = model.CustomDisplayName;
            Sector1Time = model.Sector1Time;
            Sector2Time = model.Sector2Time;
            Sector3Time = model.Sector3Time;
        }

        public override LapSummaryDto SaveToNewModel()
        {
           throw new NotImplementedException();
        }
    }
}