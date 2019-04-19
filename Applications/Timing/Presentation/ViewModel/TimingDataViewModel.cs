namespace SecondMonitor.Timing.Presentation.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Threading;
    using WindowsControls.SituationOverview;
    using Commands;
    using DataModel.BasicProperties;
    using DataModel.Snapshot;

    using PluginManager.GameConnector;

    using DataModel.Extensions;
    using Controllers;
    using DataModel.Snapshot.Drivers;
    using SecondMonitor.Timing.LapTimings.ViewModel;
    using SecondMonitor.Timing.ReportCreation.ViewModel;
    using SecondMonitor.Timing.SessionTiming.Drivers.Presentation.ViewModel;
    using SecondMonitor.Timing.SessionTiming.Drivers.ViewModel;
    using SecondMonitor.Timing.SessionTiming.ViewModel;
    using ViewModels;
    using ViewModels.CarStatus;
    using ViewModels.TrackInfo;

    using SessionTiming.Drivers;
    using SimdataManagement.DriverPresentation;
    using Telemetry;
    using ViewModels.Controllers;
    using ViewModels.Settings.Model;
    using ViewModels.Settings.ViewModel;

    public class TimingDataViewModel : DependencyObject, ISimulatorDataSetViewModel,  INotifyPropertyChanged, IPaceProvider
    {
        private static readonly DependencyProperty CurrentSessionOptionsViewProperty = DependencyProperty.Register("CurrentSessionOptionsView", typeof(SessionOptionsViewModel), typeof(TimingDataViewModel), new PropertyMetadata(null, CurrentSessionOptionsPropertyChanged));
        private static readonly DependencyProperty SelectedDriverTimingViewModelProperty = DependencyProperty.Register("SelectedDriverTimingViewModel", typeof(DriverTimingViewModel), typeof(TimingDataViewModel));
        private static readonly DependencyProperty OpenCarSettingsCommandProperty = DependencyProperty.Register("OpenCarSettingsCommand", typeof(ICommand), typeof(TimingDataViewModel));
        private readonly DriverLapsWindowManager _driverLapsWindowManager;
        private readonly DriverPresentationsManager _driverPresentationsManager;
        private readonly ISessionTelemetryControllerFactory _sessionTelemetryControllerFactory;

        private ICommand _resetCommand;

        private TimingDataViewModelResetModeEnum _shouldReset = TimingDataViewModelResetModeEnum.NoReset;

        private SessionTiming _sessionTiming;
        private SessionType _sessionType = SessionType.Na;
        private SimulatorDataSet _lastDataSet;
        private bool _isOpenCarSettingsCommandEnable;

        private bool _isNamesNotUnique;
        private string _notUniqueNamesMessage;
        private Stopwatch _notUniqueCheckWatch;

        private Task _refreshGuiTask;
        private Task _refreshBasicInfoTask;
        private Task _refreshTimingCircleTask;
        private Task _refreshTimingGridTask;

        private string _connectedSource;
        private MapManagementController _mapManagementController;
        private DisplaySettingsViewModel _displaySettingsViewModel;


        public TimingDataViewModel(DriverLapsWindowManager driverLapsWindowManager, DisplaySettingsViewModel displaySettingsViewModel, DriverPresentationsManager driverPresentationsManager, ISessionTelemetryControllerFactory sessionTelemetryControllerFactory)
        {
            TimingDataGridViewModel = new TimingDataGridViewModel(driverPresentationsManager);
            SessionInfoViewModel = new SessionInfoViewModel();
            TrackInfoViewModel = new TrackInfoViewModel();
            _driverLapsWindowManager = driverLapsWindowManager;
            _driverPresentationsManager = driverPresentationsManager;
            _sessionTelemetryControllerFactory = sessionTelemetryControllerFactory;
            DoubleLeftClickCommand = _driverLapsWindowManager.OpenWindowCommand;
            ReportsController = new ReportsController(DisplaySettingsViewModel);
            DisplaySettingsViewModel = displaySettingsViewModel;
            SituationOverviewProvider = new SituationOverviewProvider(TimingDataGridViewModel, displaySettingsViewModel);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TimeSpan? PlayersPace => SessionTiming?.Player?.Pace;
        public TimeSpan? LeadersPace => SessionTiming?.Leader?.Pace;

        public DisplaySettingsViewModel DisplaySettingsViewModel
        {
            get => _displaySettingsViewModel;
            private set
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

        public bool IsNamesNotUnique
        {
            get => _isNamesNotUnique;
            private set
            {
                _isNamesNotUnique = value;
                NotifyPropertyChanged();
            }
        }

        public string NotUniqueNamesMessage
        {
            get => _notUniqueNamesMessage;
            private set
            {
                _notUniqueNamesMessage = value;
                NotifyPropertyChanged();
            }
        }

        public TimingDataGridViewModel TimingDataGridViewModel { get; }

        public int SessionCompletedPercentage => _sessionTiming?.SessionCompletedPerMiles ?? 50;

        public ICommand ResetCommand => _resetCommand ?? (_resetCommand = new NoArgumentCommand(ScheduleReset));

        public ICommand OpenSettingsCommand { get; set; }

        public ICommand RightClickCommand { get; set; }

        public ICommand ScrollToPlayerCommand { get; set; }

        public ICommand OpenCurrentTelemetrySession { get; set; }

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

        public ReportsController ReportsController { get; }

        public string SessionTime => _sessionTiming?.SessionTime.FormatToDefault() ?? string.Empty;

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
            get => _sessionTiming;
            private set
            {
                _sessionTiming = value;
                NotifyPropertyChanged();
            }
        }

        public CarStatusViewModel CarStatusViewModel
        {
            get;
            private set;
        }

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

            if (_refreshTimingGridTask.IsFaulted && _refreshTimingGridTask.Exception != null)
            {
                exceptions.AddRange(_refreshTimingGridTask.Exception.InnerExceptions);
            }
        }

        public void ApplyDateSet(SimulatorDataSet data)
        {
            if (data == null)
            {
                return;
            }

            _lastDataSet = data;
            IsOpenCarSettingsCommandEnable = !string.IsNullOrWhiteSpace(data?.PlayerInfo?.CarName);
            ConnectedSource = _lastDataSet?.Source;
            if (_sessionTiming == null)
            {
                return;
            }

            if (_sessionType != data.SessionInfo.SessionType)
            {
                _shouldReset = TimingDataViewModelResetModeEnum.Automatic;
                _sessionType = _sessionTiming.SessionType;
            }

            // Reset state was detected (either reset button was pressed or timing detected a session change)
            if (_shouldReset != TimingDataViewModelResetModeEnum.NoReset)
            {
                CreateTiming(data);
                _shouldReset = TimingDataViewModelResetModeEnum.NoReset;
            }

            try
            {
                CheckNamesUniques(data);
                _sessionTiming?.UpdateTiming(data);
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
            CarStatusViewModel = new CarStatusViewModel(this);
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
            _refreshGuiTask = SchedulePeriodicAction(() => RefreshGui(_lastDataSet), () => 10000, this, true);
            _refreshBasicInfoTask = SchedulePeriodicAction(() => RefreshBasicInfo(_lastDataSet), () => 100, this, true);
            _refreshTimingCircleTask = SchedulePeriodicAction(() => RefreshTimingCircle(_lastDataSet), () => 100, this, true);
            _refreshTimingGridTask = SchedulePeriodicAction(() => RefreshTimingGrid(_lastDataSet), () => DisplaySettingsViewModel.RefreshRate, this, false);
        }

        private void RefreshTimingGrid(SimulatorDataSet lastDataSet)
        {
            TimingDataGridViewModel.UpdateProperties();
        }

        private void PaceLapsChanged()
        {
            if (_sessionTiming != null)
            {
                _sessionTiming.PaceLaps = DisplaySettingsViewModel.PaceLaps;
            }

        }

        private void CheckNamesUniques(SimulatorDataSet dataSet)
        {
            if (_notUniqueCheckWatch == null || _notUniqueCheckWatch.ElapsedMilliseconds < 10000)
            {
                return;
            }

            List<IGrouping<string, string>> namesGrouping = dataSet.DriversInfo.Select(x => x.DriverName).GroupBy(x => x).ToList();

            List<string> uniqueNames = namesGrouping.Where(x => x.Count() == 1).SelectMany(x => x).ToList();
            List<string> notUniqueNames = namesGrouping.Where(x => x.Count() > 1).Select(x => x.Key).ToList();


            if (notUniqueNames.Count == 0)
            {
                IsNamesNotUnique = false;
                return;
            }

            IsNamesNotUnique = true;
            NotUniqueNamesMessage = $"Not All Driver Names are unique: Number of unique drivers - {uniqueNames.Count}, Not unique names - {string.Join(", ", notUniqueNames)} ";
            _notUniqueCheckWatch.Restart();
        }

        private void ScheduleReset()
        {
            _shouldReset = TimingDataViewModelResetModeEnum.Manual;
        }

        private void ChangeOrderingMode()
        {
            if (_sessionTiming == null)
            {
                return;
            }

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(ChangeOrderingMode);
                return;
            }

            var mode = GetOrderTypeFromSettings();
            TimingDataGridViewModel.DriversOrdering = mode;
            _sessionTiming.DisplayGapToPlayerRelative = mode != DisplayModeEnum.Absolute;

        }

        private void ChangeTimeDisplayMode()
        {
            if (_sessionTiming == null || GuiDispatcher == null)
            {
                return;
            }

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(ChangeTimeDisplayMode);
                return;
            }

            var mode = GetTimeDisplayTypeFromSettings();
            _sessionTiming.DisplayBindTimeRelative = mode == DisplayModeEnum.Relative;
            _sessionTiming.DisplayGapToPlayerRelative = mode == DisplayModeEnum.Relative;
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

        private void SessionTimingDriverRemoved(object sender, DriverListModificationEventArgs e)
        {
            if (TerminatePeriodicTasks)
            {
                return;
            }
            TimingDataGridViewModel.RemoveDriver(e.Data);

            GuiDispatcher?.Invoke(() =>
            {
                SituationOverviewProvider.RemoveDriver(e.Data.DriverInfo);
            });

        }

        private void SessionTimingDriverAdded(object sender, DriverListModificationEventArgs e)
        {

            TimingDataGridViewModel.AddDriver(e.Data);

            GuiDispatcher?.Invoke(() =>
            {
                SituationOverviewProvider.AddDriver(e.Data.DriverInfo);
            });
            _driverLapsWindowManager.Rebind(e.Data);
        }

        private void RefreshGui(SimulatorDataSet data)
        {
            if (data == null)
            {
                return;
            }

            ScrollToPlayerCommand?.Execute(null);
        }

        private void CreateTiming(SimulatorDataSet data)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => CreateTiming(data));
                return;
            }

            bool invalidateLap = _shouldReset == TimingDataViewModelResetModeEnum.Manual ||
                                data.SessionInfo.SessionType != SessionType.Race;
            _lastDataSet = data;
            if (_sessionTiming != null && ReportsController != null)
            {
                ReportsController.CreateReport(_sessionTiming);
            }

            SessionTiming = SessionTiming.FromSimulatorData(data, invalidateLap, this, _driverPresentationsManager, _sessionTelemetryControllerFactory);
            foreach (var driverTimingModelView in SessionTiming.Drivers.Values)
            {
                _driverLapsWindowManager.Rebind(driverTimingModelView);
            }

            SessionInfoViewModel.SessionTiming = _sessionTiming;
            SessionTiming.DriverAdded += SessionTimingDriverAdded;
            SessionTiming.DriverRemoved += SessionTimingDriverRemoved;
            SessionTiming.PaceLaps = DisplaySettingsViewModel.PaceLaps;

            CarStatusViewModel.Reset();
            TrackInfoViewModel.Reset();
            SituationOverviewProvider.Reset();
            if (_mapManagementController != null)
            {
                _mapManagementController.SessionTiming = _sessionTiming;
            }

            InitializeGui(data);
            ChangeTimeDisplayMode();
            ChangeOrderingMode();
            ConnectedSource = data.Source;
            _notUniqueCheckWatch = Stopwatch.StartNew();
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

            TimingDataGridViewModel.MatchDriversList(_sessionTiming.Drivers.Values.ToList());
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

        private static async Task SchedulePeriodicAction(Action action, Func<int> delayFunc, TimingDataViewModel sender, bool captureContext)
        {
            while (!sender.TerminatePeriodicTasks)
            {
                await Task.Delay(delayFunc(), CancellationToken.None).ConfigureAwait(captureContext);

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
