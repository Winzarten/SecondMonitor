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
        private int _lapNumber;
        private bool _display;
        private Color _lapColor;

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