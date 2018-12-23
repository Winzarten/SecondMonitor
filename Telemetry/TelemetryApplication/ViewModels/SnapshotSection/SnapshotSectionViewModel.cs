namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.SnapshotSection
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using DataModel.BasicProperties;
    using Factory;
    using Replay;
    using SecondMonitor.ViewModels;
    using SecondMonitor.ViewModels.CarStatus;
    using TelemetryManagement.DTO;

    public class SnapshotSectionViewModel : AbstractViewModel, ISnapshotSectionViewModel
    {

        private readonly List<LapSummaryDto> _availableLaps;

        private LapSummaryDto _selectedLap;
        private TemperatureUnits _temperatureUnits;
        private PressureUnits _pressureUnits;

        public SnapshotSectionViewModel(IViewModelFactory viewModelFactory)
        {
            _availableLaps = new List<LapSummaryDto>();
            ReplayViewModel = viewModelFactory.Create<IReplayViewModel>();
            PedalSectionViewModel = viewModelFactory.Create<IPedalSectionViewModel>();
            CarWheelsViewModel = new CarWheelsViewModel();

        }

        public ReadOnlyCollection<LapSummaryDto> AvailableLaps => _availableLaps.AsReadOnly();

        public LapSummaryDto SelectedLap
        {
            get => _selectedLap;
            set
            {
                _selectedLap = value;
                NotifyPropertyChanged();
            }
        }

        public CarWheelsViewModel CarWheelsViewModel { get; }

        public TemperatureUnits TemperatureUnits
        {
            get => _temperatureUnits;
            set
            {
                _temperatureUnits = value;
                NotifyPropertyChanged();
            }
        }

        public PressureUnits PressureUnits
        {
            get => _pressureUnits;
            set
            {
                _pressureUnits = value;
                NotifyPropertyChanged();
            }
        }

        public IReplayViewModel ReplayViewModel { get; }

        public IPedalSectionViewModel PedalSectionViewModel { get; }

        public void AddAvailableLap(LapSummaryDto lapSummaryDto)
        {
            _availableLaps.Add(lapSummaryDto);
            if (_availableLaps.Count == 1)
            {
                SelectedLap = lapSummaryDto;
            }

            _availableLaps.Sort((x,y) => x.LapNumber.CompareTo(y.LapNumber));
            NotifyPropertyChanged(nameof(AvailableLaps));
        }

        public void RemoveAvailableLap(LapSummaryDto lapSummaryDto)
        {
            _availableLaps.RemoveAll(x => x == lapSummaryDto);
            if (lapSummaryDto == SelectedLap)
            {
                SelectedLap = _availableLaps.Any() ? _availableLaps.First() : null;
            }
        }

        public void ClearAvailableLaps()
        {
            _availableLaps.Clear();
            SelectedLap = null;
            NotifyPropertyChanged(nameof(AvailableLaps));

        }
    }
}