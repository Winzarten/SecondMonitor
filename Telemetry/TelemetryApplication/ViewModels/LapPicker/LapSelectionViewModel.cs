namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.LapPicker
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Controllers.Synchronization;
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

        public event EventHandler<LapSummaryArgs> LapSelected;
        public event EventHandler<LapSummaryArgs> LapUnselected;

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
            lapSummaryViewModel.PropertyChanged+= LapSummaryViewModelOnPropertyChanged;
            LapSummaries.Add(lapSummaryViewModel);
        }

        public void Clear()
        {
            foreach (ILapSummaryViewModel lapSummaryViewModel in LapSummaries)
            {
                lapSummaryViewModel.PropertyChanged -= LapSummaryViewModelOnPropertyChanged;
            }

            LapSummaries.Clear();
        }

        private void LapSummaryViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ILapSummaryViewModel.Selected) || !(sender is ILapSummaryViewModel lapSummaryViewModel))
            {
                return;
            }

            if (lapSummaryViewModel.Selected)
            {
                LapSelected?.Invoke(this, new LapSummaryArgs(lapSummaryViewModel.OriginalModel));
            }
            else
            {
                LapUnselected?.Invoke(this, new LapSummaryArgs(lapSummaryViewModel.OriginalModel));
            }
        }
    }
}