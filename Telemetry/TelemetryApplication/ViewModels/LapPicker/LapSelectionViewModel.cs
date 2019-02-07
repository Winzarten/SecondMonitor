namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.LapPicker
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Controllers.Synchronization;
    using OpenWindow;
    using SecondMonitor.ViewModels;
    using SettingsWindow;

    public class LapSelectionViewModel : AbstractViewModel, ILapSelectionViewModel
    {
        private ILapSummaryViewModel _selected;
        private string _carName;
        private string _trackName;
        private DateTime _sessionTime;
        private string _simulatorName;
        private string _bestLap;
        private IOpenWindowViewModel _openWindowViewModel;
        private ISettingsWindowViewModel _settingsWindowViewModel;
        private string _bestSector1;
        private string _bestSector2;
        private string _bestSector3;

        public LapSelectionViewModel()
        {
            LapSummaries = new ObservableCollection<ILapSummaryViewModel>();
        }

        public event EventHandler<LapSummaryArgs> LapSelected;
        public event EventHandler<LapSummaryArgs> LapUnselected;

        public IOpenWindowViewModel OpenWindowViewModel
        {
            get => _openWindowViewModel;
            set => SetProperty(ref _openWindowViewModel, value);
        }

        public ISettingsWindowViewModel SettingsWindowViewModel
        {
            get => _settingsWindowViewModel;
            set => SetProperty(ref _settingsWindowViewModel, value);
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

        public string BestLap
        {
            get => _bestLap;
            set
            {
                _bestLap = value;
                NotifyPropertyChanged();
            }
        }

        public string BestSector1
        {
            get => _bestSector1;
            set => SetProperty(ref _bestSector1, value);
        }

        public string BestSector2
        {
            get => _bestSector2;
            set => SetProperty(ref _bestSector2, value);
        }

        public string BestSector3
        {
            get => _bestSector3;
            set => SetProperty(ref _bestSector3, value);
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