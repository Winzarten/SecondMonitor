namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.LapPicker
{
    using System;
    using System.Collections.ObjectModel;
    using SecondMonitor.ViewModels;

    public class LapSelectionViewModel : AbstractViewModel, ILapSelectionViewModel
    {
        private ILapSummaryViewModel _selected;
        private string _carName;
        private string _trackName;
        private DateTime _sessionTime;
        private string _simulatorName;

        public LapSelectionViewModel()
        {
            LapSummaries = new ObservableCollection<ILapSummaryViewModel>();
        }

        public ObservableCollection<ILapSummaryViewModel> LapSummaries { get; }

        public ILapSummaryViewModel Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                NotifyPropertyChanged();
            }
        }

        public string CarName
        {
            get =>_carName;
            set
            {
                _carName = value;
                NotifyPropertyChanged();
            }
        }

        public string TrackName
        {
            get => _trackName;
            set
            {
                _trackName = value;
                NotifyPropertyChanged();
            }
        }

        public string SimulatorName
        {
            get => _simulatorName;
            set
            {
                _simulatorName = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime SessionTime
        {
            get => _sessionTime;
            set
            {
                _sessionTime = value;
                NotifyPropertyChanged();
            }
        }

        public void AddLapSummaryViewModel(ILapSummaryViewModel lapSummaryViewModel)
        {
            LapSummaries.Add(lapSummaryViewModel);
        }

        public void Clear()
        {
            LapSummaries.Clear();
        }
    }
}