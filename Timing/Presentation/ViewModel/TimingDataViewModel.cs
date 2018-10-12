﻿namespace SecondMonitor.Timing.Presentation.ViewModel
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

    using SecondMonitor.DataModel.Extensions;
    using SecondMonitor.Timing.Controllers;
    using SecondMonitor.Timing.LapTimings.ViewModel;
    using SecondMonitor.Timing.ReportCreation.ViewModel;
    using SecondMonitor.Timing.SessionTiming.Drivers.Presentation.ViewModel;
    using SecondMonitor.Timing.SessionTiming.Drivers.ViewModel;
    using SecondMonitor.Timing.SessionTiming.ViewModel;
    using SecondMonitor.Timing.Settings.ViewModel;
    using SecondMonitor.ViewModels;
    using SecondMonitor.ViewModels.CarStatus;
    using SecondMonitor.ViewModels.SituationOverview;
    using SecondMonitor.ViewModels.TrackInfo;

    using SessionTiming.Drivers;

    using Settings.Model;

    public class TimingDataViewModel : DependencyObject, ISimulatorDataSetViewModel,  INotifyPropertyChanged
    {

        private static readonly DependencyProperty DisplaySettingsViewModelProperty = DependencyProperty.Register("DisplaySettingsView", typeof(DisplaySettingsViewModel), typeof(TimingDataViewModel), new PropertyMetadata(null, PropertyChangedCallback));
        private static readonly DependencyProperty CurrentSessionOptionsViewProperty = DependencyProperty.Register("CurrentSessionOptionsView", typeof(SessionOptionsViewModel), typeof(TimingDataViewModel), new PropertyMetadata(null, CurrentSessionOptionsPropertyChanged));
        private static readonly DependencyProperty SelectedDriverTimingViewModelProperty = DependencyProperty.Register("SelectedDriverTimingViewModel", typeof(DriverTimingViewModel), typeof(TimingDataViewModel));
        private static readonly DependencyProperty OpenCarSettingsCommandProperty = DependencyProperty.Register("OpenCarSettingsCommand", typeof(ICommand), typeof(TimingDataViewModel));
        private static readonly DependencyProperty OpenCarSettingsCommandEnabledProperty = DependencyProperty.Register("IsOpenCarSettingsCommandEnable", typeof(bool), typeof(TimingDataViewModel));
        private readonly DriverLapsWindowManager _driverLapsWindowManager;

        private ICommand _resetCommand;

        private TimingDataViewModelResetModeEnum _shouldReset = TimingDataViewModelResetModeEnum.NoReset;

        private SessionTiming _timing;
        private SessionType _sessionType = SessionType.Na;
        private SimulatorDataSet _lastDataSet;

        private Task _refreshGuiTask;
        private Task _refreshBasicInfoTask;
        private Task _refreshTimingCircleTask;

        private string _connectedSource;

        public TimingDataViewModel(DriverLapsWindowManager driverLapsWindowManager)
        {
            SessionInfoViewModel = new SessionInfoViewModel();
            TrackInfoViewModel = new TrackInfoViewModel();
            _driverLapsWindowManager = driverLapsWindowManager;
            DoubleLeftClickCommand = _driverLapsWindowManager.OpenWindowCommand;
            ReportsController = new ReportsController(DisplaySettingsViewModel);
            SituationOverviewProvider = new SituationOverviewProvider(SessionTiming);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DisplaySettingsViewModel DisplaySettingsViewModel
        {
            get => (DisplaySettingsViewModel)GetValue(DisplaySettingsViewModelProperty);
            set => SetValue(DisplaySettingsViewModelProperty, value);
        }

        public SessionOptionsViewModel CurrentSessionOptionsView
        {
            get => (SessionOptionsViewModel)GetValue(CurrentSessionOptionsViewProperty);
            set => SetValue(CurrentSessionOptionsViewProperty, value);
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
            get => (bool)GetValue(OpenCarSettingsCommandEnabledProperty);
            set => SetValue(OpenCarSettingsCommandEnabledProperty, value);
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
            if (GuiDispatcher == null)
            {
                return;
            }

            if (Dispatcher.CheckAccess())
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
            else
            {
                Dispatcher.Invoke(() => ApplyDateSet(data));
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
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(Reset);
                return;
            }

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
            _refreshGuiTask = SchedulePeriodicAction(() => RefreshGui(_lastDataSet), 10000, this);
            _refreshBasicInfoTask = SchedulePeriodicAction(() => RefreshBasicInfo(_lastDataSet), 100, this);
            _refreshTimingCircleTask = SchedulePeriodicAction(() => RefreshTimingCircle(_lastDataSet), 100, this);
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
            if (data == null || GuiDispatcher == null)
            {
                return;
            }

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => RefreshBasicInfo(data));
                return;
            }

            NotifyPropertyChanged(nameof(SessionTime));
            NotifyPropertyChanged(nameof(SystemTime));
            NotifyPropertyChanged(nameof(SessionCompletedPercentage));
            CarStatusViewModel.ApplyDateSet(data);
            TrackInfoViewModel.ApplyDateSet(data);
            SessionInfoViewModel.ApplyDateSet(data);
            //SituationOverviewProvider.ApplyDateSet(data);
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
            if (data == null || GuiDispatcher == null)
            {
                return;
            }

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => RefreshGui(data));
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

            SessionTiming = SessionTiming.FromSimulatorData(data, invalidateLap, this);
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

            InitializeGui(data);
            ChangeTimeDisplayMode();
            ChangeOrderingMode();
            ConnectedSource = data.Source;
            //NotifyPropertyChanged("BestLapFormatted");
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

            //NotifyPropertyChanged("BestLapFormatted");
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnDisplaySettingsChange(object sender, PropertyChangedEventArgs args)
        {
            ApplyDisplaySettings(DisplaySettingsViewModel);
            if (args?.PropertyName == "PaceLaps")
            {
                PaceLapsChanged();
            }

            if (args?.PropertyName == SessionOptionsViewModel.OrderingModeProperty.Name)
            {
                ChangeOrderingMode();
            }

            if (args?.PropertyName == SessionOptionsViewModel.TimesDisplayModeProperty.Name)
            {
                ChangeTimeDisplayMode();
            }
        }

        private void ApplyDisplaySettings(DisplaySettingsViewModel settingsView)
        {
            TrackInfoViewModel.TemperatureUnits = settingsView.TemperatureUnits;
            SituationOverviewProvider.AnimateDriversPos = settingsView.AnimateDriversPosition;
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            TimingDataViewModel timingDataViewModel = (TimingDataViewModel) dependencyObject;
            DisplaySettingsViewModel newDisplaySettingsViewModel =
                (DisplaySettingsViewModel) dependencyPropertyChangedEventArgs.NewValue;
            newDisplaySettingsViewModel.PropertyChanged += timingDataViewModel.OnDisplaySettingsChange;
            newDisplaySettingsViewModel.PracticeSessionDisplayOptionsView.PropertyChanged += timingDataViewModel.OnDisplaySettingsChange;
            newDisplaySettingsViewModel.RaceSessionDisplayOptionsView.PropertyChanged += timingDataViewModel.OnDisplaySettingsChange;
            newDisplaySettingsViewModel.QualificationSessionDisplayOptionsView.PropertyChanged += timingDataViewModel.OnDisplaySettingsChange;

            if (timingDataViewModel.ReportsController != null)
            {
                timingDataViewModel.ReportsController.SettingsView = newDisplaySettingsViewModel;
            }
        }

        private static async Task SchedulePeriodicAction(Action action, int periodInMs, TimingDataViewModel sender)
        {

            while (!sender.TerminatePeriodicTasks)
            {
                await Task.Delay(periodInMs, CancellationToken.None);

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
