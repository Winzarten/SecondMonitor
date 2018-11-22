namespace SecondMonitor.Timing.Presentation.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Commands;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;

    using PluginManager.GameConnector;

    using DataModel.Extensions;
    using Controllers;
    using SecondMonitor.Timing.LapTimings.ViewModel;
    using SecondMonitor.Timing.ReportCreation.ViewModel;
    using SecondMonitor.Timing.SessionTiming.Drivers.Presentation.ViewModel;
    using SecondMonitor.Timing.SessionTiming.Drivers.ViewModel;
    using SecondMonitor.Timing.SessionTiming.ViewModel;
    using ViewModels;
    using ViewModels.CarStatus;
    using ViewModels.SituationOverview;
    using ViewModels.TrackInfo;

    using SessionTiming.Drivers;
    using SimdataManagement.DriverPresentation;
    using ViewModels.Settings.Model;
    using ViewModels.Settings.ViewModel;

    public class TimingDataViewModel : DependencyObject, ISimulatorDataSetViewModel,  INotifyPropertyChanged
    {
        private static readonly DependencyProperty CurrentSessionOptionsViewProperty = DependencyProperty.Register("CurrentSessionOptionsView", typeof(SessionOptionsViewModel), typeof(TimingDataViewModel), new PropertyMetadata(null, CurrentSessionOptionsPropertyChanged));
        private static readonly DependencyProperty SelectedDriverTimingViewModelProperty = DependencyProperty.Register("SelectedDriverTimingViewModel", typeof(DriverTimingViewModel), typeof(TimingDataViewModel));
        private static readonly DependencyProperty OpenCarSettingsCommandProperty = DependencyProperty.Register("OpenCarSettingsCommand", typeof(ICommand), typeof(TimingDataViewModel));
        private readonly DriverLapsWindowManager _driverLapsWindowManager;
        private readonly DriverPresentationsManager _driverPresentationsManager;

        private ICommand _resetCommand;

        private TimingDataViewModelResetModeEnum _shouldReset = TimingDataViewModelResetModeEnum.NoReset;

        private SessionTiming _timing;
        private SessionType _sessionType = SessionType.Na;
        private SimulatorDataSet _lastDataSet;
        private bool _isOpenCarSettingsCommandEnable;

        private Task _refreshGuiTask;
        private Task _refreshBasicInfoTask;
        private Task _refreshTimingCircleTask;

        private string _connectedSource;
        private MapManagementController _mapManagementController;
        private DisplaySettingsViewModel _displaySettingsViewModel;

        public TimingDataViewModel(DriverLapsWindowManager driverLapsWindowManager, DisplaySettingsViewModel displaySettingsViewModel, DriverPresentationsManager driverPresentationsManager)
        {
            SessionInfoViewModel = new SessionInfoViewModel();
            TrackInfoViewModel = new TrackInfoViewModel();
            _driverLapsWindowManager = driverLapsWindowManager;
            _driverPresentationsManager = driverPresentationsManager;
            DoubleLeftClickCommand = _driverLapsWindowManager.OpenWindowCommand;
            ReportsController = new ReportsController(DisplaySettingsViewModel);
            DisplaySettingsViewModel = displaySettingsViewModel;
            SituationOverviewProvider = new SituationOverviewProvider(SessionTiming, displaySettingsViewModel);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DisplaySettingsViewModel DisplaySettingsViewModel
        {
            get => _displaySettingsViewModel;
            set
            {
                _displaySettingsViewModel = value;
                NotifyPropertyChanged();
                DisplaySettingsChanged();
            }
        }

        public SessionOptionsViewModel CurrentSessionOptionsView
        {
            get => (SessionOptionsViewModel)GetValue(CurrentSessionOptionsViewProperty);
            set => SetValue(CurrentSessionOptionsViewProperty, value);
        }

        public MapManagementController MapManagementController
        {
            set
            {
                _mapManagementController = value;
                SituationOverviewProvider.MapManagementController = value;
                value.SessionTiming = SessionTiming;
            }
        }

        public int SessionCompletedPercentage => _timing?.SessionCompletedPerMiles ?? 50;

        public ICommand ResetCommand => _resetCommand ?? (_resetCommand = new NoArgumentCommand(ScheduleReset));

        public ICommand OpenSettingsCommand { get; set; }

        public ICommand RightClickCommand { get; set; }

        public ICommand ScrollToPlayerCommand { get; set; }

        public ICommand OpenCarSettingsCommand
        {
            get => (ICommand)GetValue(OpenCarSettingsCommandProperty);
            set => SetValue(OpenCarSettingsCommandProperty, value);
        }

        public bool IsOpenCarSettingsCommandEnable
        {
            get => _isOpenCarSettingsCommandEnable;
            set
            {
                _isOpenCarSettingsCommandEnable = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand DoubleLeftClickCommand
        {
            get;
            set;
        }

        // Gets or sets the CollectionViewSource
        public CollectionViewSource ViewSource { get; set; }

        // Gets or sets the ObservableCollection
        public ObservableCollection<DriverTimingViewModel> Collection { get; set; } = new ObservableCollection<DriverTimingViewModel>();

        public ReportsController ReportsController { get; }

        public string SessionTime => _timing?.SessionTime.FormatToDefault() ?? string.Empty;

        public string ConnectedSource
        {
            get => _connectedSource;
            private set
            {
                bool wasChanged = _connectedSource != value;
                _connectedSource = value;
                if (wasChanged)
                {
                    NotifyPropertyChanged();
                }
            }
        }

        public string SystemTime => DateTime.Now.ToString("HH:mm");

        public string Title
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string version = AssemblyName.GetAssemblyName(assembly.Location).Version.ToString();
                return "Second Monitor (Timing)(v" + version + " )";
            }
        }

        public SessionInfoViewModel SessionInfoViewModel { get; }

        public TrackInfoViewModel TrackInfoViewModel { get; }

        public SituationOverviewProvider SituationOverviewProvider { get; }

        public Dispatcher GuiDispatcher { get; set; }

        public DriverTimingViewModel SelectedDriverTimingViewModel
        {
            get => (DriverTimingViewModel)GetValue(SelectedDriverTimingViewModelProperty);
            set => SetValue(SelectedDriverTimingViewModelProperty, value);
        }

        public DriverTiming SelectedDriverTiming => SelectedDriverTimingViewModel?.DriverTiming;

        public SessionTiming SessionTiming
        {
            get => _timing;
            private set
            {
                _timing = value;
                NotifyPropertyChanged();
            }
        }

        public CarStatusViewModel CarStatusViewModel
        {
            get;
            private set;
        }

        public ICollectionView TimingInfo => ViewSource?.View;

        private bool TerminatePeriodicTasks { get; set; }

        public void TerminatePeriodicTask(List<Exception> exceptions)
        {
            TerminatePeriodicTasks = true;
            if (_refreshBasicInfoTask.IsFaulted && _refreshBasicInfoTask.Exception != null)
            {
                exceptions.AddRange(_refreshBasicInfoTask.Exception.InnerExceptions);
            }

            if (_refreshGuiTask.IsFaulted && _refreshGuiTask.Exception != null)
            {
                exceptions.AddRange(_refreshGuiTask.Exception.InnerExceptions);
            }

            if (_refreshTimingCircleTask.IsFaulted && _refreshTimingCircleTask.Exception != null)
            {
                exceptions.AddRange(_refreshTimingCircleTask.Exception.InnerExceptions);
            }
        }

        public void ApplyDateSet(SimulatorDataSet data)
        {



                _lastDataSet = data;
                IsOpenCarSettingsCommandEnable = !string.IsNullOrWhiteSpace(data?.PlayerInfo?.CarName);
                ConnectedSource = _lastDataSet?.Source;
                if (ViewSource == null || _timing == null)
                {
                    return;
                }

                if (_sessionType != data.SessionInfo.SessionType)
                {
                    _shouldReset = TimingDataViewModelResetModeEnum.Automatic;
                    _sessionType = _timing.SessionType;
                }

                // Reset state was detected (either reset button was pressed or timing detected a session change)
                if (_shouldReset != TimingDataViewModelResetModeEnum.NoReset)
                {
                    CreateTiming(data);
                    _shouldReset = TimingDataViewModelResetModeEnum.NoReset;
                }

                try
                {
                    _timing?.UpdateTiming(data);
                    CarStatusViewModel?.PedalsAndGearViewModel?.ApplyDateSet(data);
                }
                catch (SessionTiming.DriverNotFoundException)
                {
                    _shouldReset = TimingDataViewModelResetModeEnum.Automatic;
                }

        }

        public void DisplayMessage(MessageArgs e)
        {
            if (e.IsDecision)
            {
                if (MessageBox.Show(
                        e.Message,
                        "Message from connector.",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    e.Action();
                }
            }
            else
            {
                MessageBox.Show(e.Message, "Message from connector.", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        public void Reset()
        {
            CarStatusViewModel = new CarStatusViewModel();
            ConnectedSource = "Not Connected";

            if (GuiDispatcher != null && GuiDispatcher.CheckAccess())
            {
                GuiDispatcher.Invoke(ScheduleRefreshActions);
            }
            else
            {
                ScheduleRefreshActions();
            }

            OnDisplaySettingsChange(this, null);
            _shouldReset = TimingDataViewModelResetModeEnum.NoReset;
            new AutoUpdateController().CheckForUpdate();
        }

        private void ScheduleRefreshActions()
        {
            _refreshGuiTask = SchedulePeriodicAction(() => RefreshGui(_lastDataSet), 10000, this, true);
            _refreshBasicInfoTask = SchedulePeriodicAction(() => RefreshBasicInfo(_lastDataSet), 100, this, true);
            _refreshTimingCircleTask = SchedulePeriodicAction(() => RefreshTimingCircle(_lastDataSet), 100, this, true);
        }
        private void PaceLapsChanged()
        {
            if (_timing != null)
            {
                _timing.PaceLaps = DisplaySettingsViewModel.PaceLaps;
            }

            if (!TerminatePeriodicTasks)
            {
                GuiDispatcher.Invoke(RefreshDataGrid);
            }

        }

        private void ScheduleReset()
        {
            _shouldReset = TimingDataViewModelResetModeEnum.Manual;
        }

        private void ChangeOrderingMode()
        {
            if (ViewSource == null || _timing == null)
            {
                return;
            }

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(ChangeOrderingMode);
                return;
            }

            var mode = GetOrderTypeFromSettings();
            if (mode == DisplayModeEnum.Absolute)
            {
                ViewSource.SortDescriptions.Clear();
                ViewSource.SortDescriptions.Add(new SortDescription("DriverTiming.Position", ListSortDirection.Ascending));
                _timing.DisplayGapToPlayerRelative = false;
            }
            else
            {
                ViewSource.SortDescriptions.Clear();
                ViewSource.SortDescriptions.Add(new SortDescription("DriverTiming.DistanceToPlayer", ListSortDirection.Ascending));
                _timing.DisplayGapToPlayerRelative = true;
            }

        }

        private void ChangeTimeDisplayMode()
        {
            if (_timing == null || GuiDispatcher == null)
            {
                return;
            }

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(ChangeTimeDisplayMode);
                return;
            }

            var mode = GetTimeDisplayTypeFromSettings();
            _timing.DisplayBindTimeRelative = mode == DisplayModeEnum.Relative;
            _timing.DisplayGapToPlayerRelative = mode == DisplayModeEnum.Relative;
        }

        private DisplayModeEnum GetOrderTypeFromSettings()
        {
            return CurrentSessionOptionsView.OrderingMode;
        }

        private DisplayModeEnum GetTimeDisplayTypeFromSettings()
        {
            return CurrentSessionOptionsView.TimesDisplayMode;
        }

        private SessionOptionsViewModel GetSessionOptionOfCurrentSession(SimulatorDataSet dataSet)
        {
            if (dataSet == null)
            {
                return new SessionOptionsViewModel();
            }

            switch (dataSet.SessionInfo.SessionType)
            {
                case SessionType.Practice:
                case SessionType.WarmUp:
                    return DisplaySettingsViewModel.PracticeSessionDisplayOptionsView;
                case SessionType.Qualification:
                    return DisplaySettingsViewModel.QualificationSessionDisplayOptionsView;
                case SessionType.Race:
                    return DisplaySettingsViewModel.RaceSessionDisplayOptionsView;
                default:
                    return new SessionOptionsViewModel();
            }
        }

        private void RefreshTimingCircle(SimulatorDataSet data)
        {

            if (data == null || GuiDispatcher == null)
            {
                return;
            }

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => RefreshTimingCircle(data));
                return;
            }

            SituationOverviewProvider.ApplyDateSet(data);
        }

        private void RefreshBasicInfo(SimulatorDataSet data)
        {
            if (data == null)
            {
                return;
            }

            NotifyPropertyChanged(nameof(SessionTime));
            NotifyPropertyChanged(nameof(SystemTime));
            NotifyPropertyChanged(nameof(SessionCompletedPercentage));
            CarStatusViewModel.ApplyDateSet(data);
            TrackInfoViewModel.ApplyDateSet(data);
            SessionInfoViewModel.ApplyDateSet(data);
        }

        private void Timing_DriverRemoved(object sender, DriverListModificationEventArgs e)
        {
            if (TerminatePeriodicTasks)
            {
                return;
            }

            GuiDispatcher?.Invoke(() =>
            {
                SituationOverviewProvider.RemoveDriver(e.Data.DriverTiming.DriverInfo);
                Collection?.Remove(e.Data);
            });

        }

        private void Timing_DriverAdded(object sender, DriverListModificationEventArgs e)
        {
            GuiDispatcher?.Invoke(() =>
            {
                Collection?.Add(e.Data);
                SituationOverviewProvider.AddDriver(e.Data.DriverTiming.DriverInfo);
            });
            _driverLapsWindowManager.Rebind(e.Data.DriverTiming);
        }

        private void RefreshGui(SimulatorDataSet data)
        {
            if (data == null)
            {
                return;
            }

            RefreshDataGrid();
            ScrollToPlayerCommand?.Execute(null);
        }

        private void RefreshDataGrid()
        {
            ViewSource?.View.Refresh();
        }

        private void CreateTiming(SimulatorDataSet data)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => CreateTiming(data));
                return;
            }

            var invalidateLap = _shouldReset == TimingDataViewModelResetModeEnum.Manual ||
                                data.SessionInfo.SessionType != SessionType.Race;
            _lastDataSet = data;
            if (_timing != null && ReportsController != null)
            {
                ReportsController.CreateReport(_timing);
            }

            SessionTiming = SessionTiming.FromSimulatorData(data, invalidateLap, this, _driverPresentationsManager);
            foreach (var driverTimingModelView in _timing.Drivers.Values)
            {
                _driverLapsWindowManager.Rebind(driverTimingModelView.DriverTiming);
            }

            SituationOverviewProvider.PositionCircleInformationProvider = _timing;
            SessionInfoViewModel.SessionTiming = _timing;
            _timing.DriverAdded += Timing_DriverAdded;
            _timing.DriverRemoved += Timing_DriverRemoved;
            _timing.PaceLaps = DisplaySettingsViewModel.PaceLaps;

            CarStatusViewModel.Reset();
            TrackInfoViewModel.Reset();
            SituationOverviewProvider.Reset();
            if (_mapManagementController != null)
            {
                _mapManagementController.SessionTiming = _timing;
            }

            InitializeGui(data);
            ChangeTimeDisplayMode();
            ChangeOrderingMode();
            ConnectedSource = data.Source;
            NotifyPropertyChanged(nameof(ConnectedSource));
        }

        public void StartNewSession(SimulatorDataSet dataSet)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => StartNewSession(dataSet));
                return;
            }

            UpdateCurrentSessionOption(dataSet);
            CreateTiming(dataSet);
        }

        private void UpdateCurrentSessionOption(SimulatorDataSet data)
        {
            CurrentSessionOptionsView = GetSessionOptionOfCurrentSession(data);
        }

        private void InitializeGui(SimulatorDataSet data)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => InitializeGui(data));
                return;
            }

            if (ViewSource == null)
            {
                ViewSource = new CollectionViewSource { Source = Collection };
                NotifyPropertyChanged(nameof(TimingInfo));
            }

            Collection.Clear();
            foreach (DriverTimingViewModel d in _timing.Drivers.Values)
            {
                Collection.Add(d);
            }

            SituationOverviewProvider.ApplyDateSet(data);
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnDisplaySettingsChange(object sender, PropertyChangedEventArgs args)
        {
            ApplyDisplaySettings(DisplaySettingsViewModel);
            switch (args?.PropertyName)
            {
                case nameof(DisplaySettingsViewModel.PaceLaps):
                    PaceLapsChanged();
                    break;
                case nameof(SessionOptionsViewModel.OrderingMode):
                    ChangeOrderingMode();
                    break;
                case nameof(SessionOptionsViewModel.TimesDisplayMode):
                    ChangeTimeDisplayMode();
                    break;
            }
        }

        private void ApplyDisplaySettings(DisplaySettingsViewModel settingsView)
        {
            TrackInfoViewModel.TemperatureUnits = settingsView.TemperatureUnits;
            TrackInfoViewModel.DistanceUnits = settingsView.DistanceUnits;
            SituationOverviewProvider.DisplaySettingsViewModel  = settingsView;
        }

        private void DisplaySettingsChanged()
        {
            DisplaySettingsViewModel newDisplaySettingsViewModel = _displaySettingsViewModel;
            newDisplaySettingsViewModel.PropertyChanged += OnDisplaySettingsChange;
            newDisplaySettingsViewModel.PracticeSessionDisplayOptionsView.PropertyChanged += OnDisplaySettingsChange;
            newDisplaySettingsViewModel.RaceSessionDisplayOptionsView.PropertyChanged += OnDisplaySettingsChange;
            newDisplaySettingsViewModel.QualificationSessionDisplayOptionsView.PropertyChanged += OnDisplaySettingsChange;

            if (ReportsController != null)
            {
                ReportsController.SettingsView = newDisplaySettingsViewModel;
            }
        }

        private static async Task SchedulePeriodicAction(Action action, int periodInMs, TimingDataViewModel sender, bool captureContext)
        {

            while (!sender.TerminatePeriodicTasks)
            {
                await Task.Delay(periodInMs, CancellationToken.None).ConfigureAwait(captureContext);

                if (!sender.TerminatePeriodicTasks)
                {
                    action();
                }
            }


        }

        private static void CurrentSessionOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TimingDataViewModel timingDataViewModel)
            {
                timingDataViewModel.ChangeOrderingMode();
                timingDataViewModel.ChangeTimeDisplayMode();
            }
        }
    }
}
