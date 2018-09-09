namespace SecondMonitor.Timing.Presentation.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    using Commands;

    using DataModel.BasicProperties;
    using DataModel.Snapshot;

    using PluginManager.Core;
    using PluginManager.GameConnector;

    using SecondMonitor.Timing.LapTimings.ViewModel;
    using SecondMonitor.Timing.ReportCreation.ViewModel;
    using SecondMonitor.Timing.SessionTiming.Drivers.Presentation.ViewModel;
    using SecondMonitor.Timing.SessionTiming.Drivers.ViewModel;
    using SecondMonitor.Timing.SessionTiming.ViewModel;
    using SecondMonitor.ViewModels.CarStatus;
    using SecondMonitor.ViewModels.SituationOverview;
    using SecondMonitor.ViewModels.TrackInfo;

    using SessionTiming.Drivers;

    using Settings;
    using Settings.Model;
    using Settings.ModelView;

    using View;

    public class TimingDataViewModel : DependencyObject, ISecondMonitorPlugin, INotifyPropertyChanged
    {

        public static readonly DependencyProperty DisplaySettingsProperty = DependencyProperty.Register("DisplaySettings", typeof(DisplaySettingsModelView), typeof(TimingDataViewModel), new PropertyMetadata(null, PropertyChangedCallback));
        public static readonly DependencyProperty CurrentSessionOptionsProperty = DependencyProperty.Register("CurrentSessionOptions", typeof(SessionOptionsModelView), typeof(TimingDataViewModel), new PropertyMetadata(null, CurrentSessionOptionsPropertyChanged));
        public static readonly DependencyProperty CurrentGearProperty = DependencyProperty.Register("CurrentGear",typeof(string), typeof(TimingDataViewModel));

        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SecondMonitor\\settings.json");

        private enum ResetModeEnum
        {
            NoReset,

            Manual,

            Automatic
        }

        private ResetModeEnum _shouldReset = ResetModeEnum.NoReset;

        private DisplaySettingAutoSaver _settingAutoSaver;

        private ReportCreationViewModel _reportCreation;

        private PluginsManager _pluginsManager;
        private SessionTiming _timing;
        private DisplaySettingsWindow _settingsWindow;
        private SessionType _sessionType = SessionType.Na;
        private SimulatorDataSet _lastDataSet;

        private Task _refreshGuiTask;
        private Task _refreshBasicInfoTask;
        private Task _refreshTimingCircleTask;

        private string _connectedSource;
        private readonly DriverLapsWindowManager _driverLapsWindowManager;

        public TimingDataViewModel()
        {
            SessionInfoViewModel = new SessionInfoViewModel();
            TrackInfoViewModel = new TrackInfoViewModel();
            _driverLapsWindowManager = new DriverLapsWindowManager(() => Gui, () => SelectedDriverTiming);
            DoubleLeftClickCommand = _driverLapsWindowManager.OpenWindowCommand;
            _reportCreation = new ReportCreationViewModel(DisplaySettings);
            SituationOverviewProvider = new SituationOverviewProvider(SessionTiming);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Gets or sets the CollectionViewSource
        public CollectionViewSource ViewSource { get; set; }

        // Gets or sets the ObservableCollection
        public ObservableCollection<DriverTimingModelView> Collection { get; set; } = new ObservableCollection<DriverTimingModelView>();

        public string SessionTime => _timing?.SessionTime.ToString("mm\\:ss\\.fff") ?? string.Empty;

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

        public TimingGui Gui { get; private set; }

        public DriverTiming SelectedDriverTiming => ((DriverTimingModelView)Gui?.DtTimig.SelectedItem)?.DriverTiming;

        public SessionTiming SessionTiming
        {
            get => _timing;
            private set
            {
                _timing = value;
                NotifyPropertyChanged();
            }
        }

        public PluginsManager PluginManager
        {
            get => _pluginsManager;
            set
            {
                _pluginsManager = value;
                _pluginsManager.DataLoaded += OnDataLoaded;
                _pluginsManager.SessionStarted += OnSessionStarted;
                _pluginsManager.DisplayMessage += DisplayMessage;
            }
        }

        public CarStatusViewModel CarStatusViewModel
        {
            get;
            private set;
        }

        public ICollectionView TimingInfo => ViewSource?.View;

        public bool IsDaemon => false;

        public void RunPlugin()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(RunPlugin);
                return;
            }

            CarStatusViewModel = new CarStatusViewModel();
            ConnectedSource = "Not Connected";
            CreateDisplaySettings();
            CreateGuiInstance();
            CreateAutoSaver();

            if (Gui.Dispatcher.CheckAccess())
            {
                Gui.Dispatcher.Invoke(ScheduleRefreshActions);
            }
            else
            {
                ScheduleRefreshActions();
            }

            OnDisplaySettingsChange(this, null);
            _shouldReset = ResetModeEnum.NoReset;
        }

        private void CreateAutoSaver()
        {
            _settingAutoSaver = new DisplaySettingAutoSaver(SettingsPath);
            _settingAutoSaver.DisplaySettingsModelView = DisplaySettings;
        }

        private bool TerminatePeriodicTasks { get; set; }

        private void ScheduleRefreshActions()
        {
            _refreshGuiTask = SchedulePeriodicAction(() => RefreshGui(_lastDataSet), 10000, this);
            _refreshBasicInfoTask = SchedulePeriodicAction(() => RefreshBasicInfo(_lastDataSet), 100, this);
            _refreshTimingCircleTask = SchedulePeriodicAction(() => RefreshTimingCircle(_lastDataSet), 300, this);
        }

        private void CreateGuiInstance()
        {
            Gui = new TimingGui();
            Gui.Show();
            Gui.Closed += Gui_Closed;
            Gui.MouseLeave += GuiOnMouseLeave;
            Gui.DataContext = this;
        }

        private void GuiOnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
        {
            if (Gui != null)
            {
                Gui.DtTimig.SelectedItem = null;
            }
        }

        private void CreateDisplaySettings()
        {
            DisplaySettingsModelView displaySettingsModelView = new DisplaySettingsModelView();
            displaySettingsModelView.FromModel(
                new DisplaySettingsLoader().LoadDisplaySettingsFromFileSafe(SettingsPath));
            DisplaySettings = displaySettingsModelView;
            CurrentSessionOptions = SessionOptionsModelView.CreateFromModel(new SessionOptions());
        }

        public DisplaySettingsModelView DisplaySettings
        {
            get => (DisplaySettingsModelView) GetValue(DisplaySettingsProperty);
            set => SetValue(DisplaySettingsProperty, value);
        }

        public SessionOptionsModelView CurrentSessionOptions
        {
            get => (SessionOptionsModelView)GetValue(CurrentSessionOptionsProperty);
            set => SetValue(CurrentSessionOptionsProperty, value);
        }

        private ICommand _resetCommand;

        public ICommand ResetCommand => _resetCommand ?? (_resetCommand = new NoArgumentCommand(ScheduleReset));

        public ICommand OpenSettingsCommand => new NoArgumentCommand(OpenSettingsWindow);

        public ICommand RightClickCommand => new NoArgumentCommand(UnSelectItem);

        public ICommand DoubleLeftClickCommand
        {
            get;
            set;
        }

        private void OpenSettingsWindow()
        {
            if (_settingsWindow != null && _settingsWindow.IsVisible)
            {
                _settingsWindow.Focus();
                return;
            }

            _settingsWindow = new DisplaySettingsWindow
            {
                DataContext = DisplaySettings,
                Owner = Gui
            };
            _settingsWindow.Show();
        }

        public int SessionCompletedPercentage => _timing?.SessionCompletedPerMiles ?? 50;

        private void PaceLapsChanged()
        {
            if (_timing != null)
            {
                _timing.PaceLaps = DisplaySettings.PaceLaps;
            }

            if (!TerminatePeriodicTasks)
            {
                Gui.Dispatcher.Invoke(RefreshDataGrid);
            }

        }

        private void ScheduleReset()
        {
            _shouldReset = ResetModeEnum.Manual;
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
            if (_timing == null || Gui == null)
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
            return CurrentSessionOptions.OrderingMode;
        }

        private DisplayModeEnum GetTimeDisplayTypeFromSettings()
        {
            return CurrentSessionOptions.TimesDisplayMode;
        }

        private SessionOptionsModelView GetSessionOptionOfCurrentSession(SimulatorDataSet dataSet)
        {
            if (dataSet == null)
            {
                return new SessionOptionsModelView();
            }

            switch (dataSet.SessionInfo.SessionType)
            {
                case SessionType.Practice:
                case SessionType.WarmUp:
                    return DisplaySettings.PracticeSessionDisplayOptions;
                case SessionType.Qualification:
                    return DisplaySettings.QualificationSessionDisplayOptions;
                case SessionType.Race:
                    return DisplaySettings.RaceSessionDisplayOptions;
                default:
                    return new SessionOptionsModelView();
            }
        }

        private void Gui_Closed(object sender, EventArgs e)
        {
            Gui = null;
            TerminatePeriodicTasks = true;
            List<Exception> exceptions = new List<Exception>();
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
            _pluginsManager.DeletePlugin(this, exceptions);
        }

        private void OnDataLoaded(object sender, DataEventArgs args)
        {
            if (Gui == null)
            {
                return;
            }

            if (Dispatcher.CheckAccess())
            {

                _lastDataSet = args.Data;
                ConnectedSource = _lastDataSet?.Source;
                if (ViewSource == null || _timing == null)
                {
                    return;
                }
                SimulatorDataSet data = args.Data;

                if (_sessionType != data.SessionInfo.SessionType)
                {
                    _shouldReset = ResetModeEnum.Automatic;
                    _sessionType = _timing.SessionType;
                }

                // Reset state was detected (either reset button was pressed or timing detected a session change)
                if (_shouldReset != ResetModeEnum.NoReset)
                {
                    CreateTiming(data);
                    _shouldReset = ResetModeEnum.NoReset;
                }

                try
                {
                    _timing?.UpdateTiming(data);
                    CarStatusViewModel?.PedalsAndGearViewModel?.ApplyDateSet(data);
                }
                catch (SessionTiming.DriverNotFoundException)
                {
                    _shouldReset = ResetModeEnum.Automatic;
                }
            }
            else
            {
                Dispatcher.Invoke(() => OnDataLoaded(sender, args));
            }
        }

        private void RefreshTimingCircle(SimulatorDataSet data)
        {
            if (data == null || Gui == null)
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
            if (data == null || Gui == null)
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
            SituationOverviewProvider.ApplyDateSet(data);
        }

        private void Timing_DriverRemoved(object sender, DriverListModificationEventArgs e)
        {
            if (TerminatePeriodicTasks)
            {
                return;
            }

            Gui?.Dispatcher.Invoke(() =>
            {
                SituationOverviewProvider.RemoveDriver(e.Data.DriverTiming.DriverInfo);
                Collection?.Remove(e.Data);
            });

        }

        private void Timing_DriverAdded(object sender, DriverListModificationEventArgs e)
        {
            Gui?.Dispatcher.Invoke(() =>
            {
                Collection?.Add(e.Data);
                SituationOverviewProvider.AddDriver(e.Data.DriverTiming.DriverInfo);
            });
            _driverLapsWindowManager.Rebind(e.Data.DriverTiming);
        }

        private void RefreshGui(SimulatorDataSet data)
        {
            if (data == null || Gui == null)
            {
                return;
            }

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => RefreshGui(data));
                return;
            }

            RefreshDataGrid();

            if (DisplaySettings.ScrollToPlayer && Gui != null && _timing?.Player != null && Gui.DtTimig.Items.Count > 0)
            {
                Gui.DtTimig.ScrollIntoView(Gui.DtTimig.Items[0]);
                Gui.DtTimig.ScrollIntoView(_timing.Player);
            }

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

            var invalidateLap = _shouldReset == ResetModeEnum.Manual ||
                                data.SessionInfo.SessionType != SessionType.Race;
            _lastDataSet = data;
            if (_timing != null && _reportCreation != null)
            {
                _reportCreation.CreateReport(_timing);
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
            _timing.PaceLaps = DisplaySettings.PaceLaps;

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

        private void OnSessionStarted(object sender, DataEventArgs args)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => OnSessionStarted(sender, args));
                return;
            }

            CreateTiming(args.Data);
            UpdateCurrentSessionOption(args.Data);
        }

        private void UpdateCurrentSessionOption(SimulatorDataSet data)
        {
            CurrentSessionOptions = GetSessionOptionOfCurrentSession(data);
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
            foreach (DriverTimingModelView d in _timing.Drivers.Values)
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
            ApplyDisplaySettings(DisplaySettings);
            if (args?.PropertyName == "PaceLaps")
            {
                PaceLapsChanged();
            }

            if (args?.PropertyName == SessionOptionsModelView.OrderingModeProperty.Name)
            {
                ChangeOrderingMode();
            }

            if (args?.PropertyName == SessionOptionsModelView.TimesDisplayModeProperty.Name)
            {
                ChangeTimeDisplayMode();
            }

        }

        private void ApplyDisplaySettings(DisplaySettingsModelView settings)
        {
            TrackInfoViewModel.TemperatureUnits = settings.TemperatureUnits;
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            TimingDataViewModel timingDataViewModel = (TimingDataViewModel) dependencyObject;
            DisplaySettingsModelView newDisplaySettingsModelView =
                (DisplaySettingsModelView) dependencyPropertyChangedEventArgs.NewValue;
            newDisplaySettingsModelView.PropertyChanged += timingDataViewModel.OnDisplaySettingsChange;
            newDisplaySettingsModelView.PracticeSessionDisplayOptions.PropertyChanged += timingDataViewModel.OnDisplaySettingsChange;
            newDisplaySettingsModelView.RaceSessionDisplayOptions.PropertyChanged += timingDataViewModel.OnDisplaySettingsChange;
            newDisplaySettingsModelView.QualificationSessionDisplayOptions.PropertyChanged += timingDataViewModel.OnDisplaySettingsChange;

            if (timingDataViewModel._settingAutoSaver != null)
            {
                timingDataViewModel._settingAutoSaver.DisplaySettingsModelView = newDisplaySettingsModelView;
            }

            if (timingDataViewModel._reportCreation != null)
            {
                timingDataViewModel._reportCreation.Settings = newDisplaySettingsModelView;
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

        private void UnSelectItem()
        {
            if (Gui == null)
            {
                return;
            }

            Gui.DtTimig.SelectedItem = null;
        }


        private static void DisplayMessage(object sender, MessageArgs e)
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

    }
}
