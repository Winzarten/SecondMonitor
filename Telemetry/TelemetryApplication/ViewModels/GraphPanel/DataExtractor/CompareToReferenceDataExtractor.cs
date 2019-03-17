namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.DataExtractor
{
    using System;
    using System.Collections.Generic;
    using WindowsControls.Properties;
    using Controllers.Synchronization;
    using DataModel.BasicProperties;
    using DataModel.Extensions;
    using DataModel.Telemetry;
    using LoadedLapCache;
    using Settings.DTO;
    using TelemetryManagement.DTO;
    using TelemetryManagement.StoryBoard;

    public class CompareToReferenceDataExtractor : ISingleSeriesDataExtractor
    {
        private readonly ITelemetryViewsSynchronization _telemetryViewsSynchronization;
        private readonly TelemetryStoryBoardFactory _telemetryStoryBoardFactory;
        private readonly ILoadedLapsCache _loadedLapsCache;
        public event EventHandler<EventArgs> DataRefreshRequested;

        private readonly Dictionary<string, TelemetryStoryboard> _loadedTelemetries;
        private TelemetryStoryboard _referenceLap;

        public CompareToReferenceDataExtractor(ITelemetryViewsSynchronization telemetryViewsSynchronization, TelemetryStoryBoardFactory telemetryStoryBoardFactory, ILoadedLapsCache loadedLapsCache)
        {
            _telemetryViewsSynchronization = telemetryViewsSynchronization;
            _telemetryStoryBoardFactory = telemetryStoryBoardFactory;
            _loadedLapsCache = loadedLapsCache;
            _loadedTelemetries = new Dictionary<string, TelemetryStoryboard>();
            Subscribe();
            InitializeAlreadyLoadedLaps();
        }

        private void InitializeAlreadyLoadedLaps()
        {
            _loadedLapsCache.LoadedLaps.ForEach(AddLapLoadedLap);
            ChangeReferenceLap(_loadedLapsCache.ReferenceLap);
        }

        private void Subscribe()
        {
            _telemetryViewsSynchronization.LapLoaded += TelemetryViewsSynchronizationOnLapLoaded;
            _telemetryViewsSynchronization.LapUnloaded += TelemetryViewsSynchronizationOnLapUnloaded;
            _telemetryViewsSynchronization.ReferenceLapSelected += TelemetryViewsSynchronizationOnReferenceLapSelected;
        }

        private void TelemetryViewsSynchronizationOnReferenceLapSelected(object sender, LapSummaryArgs e)
        {
            ChangeReferenceLap(e.LapSummary);
        }

        private void ChangeReferenceLap([CanBeNull] LapSummaryDto lapSummaryDto)
        {
            _referenceLap = lapSummaryDto != null ? _loadedTelemetries[lapSummaryDto.Id] : null;
            DataRefreshRequested?.Invoke(this, new EventArgs());
        }

        private void TelemetryViewsSynchronizationOnLapUnloaded(object sender, LapSummaryArgs e)
        {
            _loadedTelemetries.Remove(e.LapSummary.Id);
        }

        private void TelemetryViewsSynchronizationOnLapLoaded(object sender, LapTelemetryArgs e)
        {
            AddLapLoadedLap(e.LapTelemetry);
        }

        private void AddLapLoadedLap(LapTelemetryDto lapTelemetryDto)
        {
            _loadedTelemetries.Add(lapTelemetryDto.LapSummary.Id, _telemetryStoryBoardFactory.Create(lapTelemetryDto));
        }


        public string ExtractorName => "Δ to Reference";

        public double GetValue(Func<TimedTelemetrySnapshot, double> valueExtractFunction, TimedTelemetrySnapshot telemetrySnapshot, XAxisKind xAxisKind)
        {
            if (_referenceLap == null)
            {
                return valueExtractFunction(telemetrySnapshot);
            }

            double toCompareValue = xAxisKind == XAxisKind.LapDistance ? _referenceLap.GetValueByDistance(Distance.FromMeters(telemetrySnapshot.PlayerData.LapDistance), valueExtractFunction) : _referenceLap.GetValueByTime(telemetrySnapshot.LapTime, valueExtractFunction);

            return valueExtractFunction(telemetrySnapshot) - toCompareValue;
        }

        public bool ShowLapGraph(LapSummaryDto lapSummaryDto)
        {
            return lapSummaryDto.Id != _referenceLap?.LapSummaryDto.Id;
        }
    }
}