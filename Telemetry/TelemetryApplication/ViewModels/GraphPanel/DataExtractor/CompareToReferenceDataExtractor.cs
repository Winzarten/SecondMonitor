namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel.DataExtractor
{
    using System;
    using System.Collections.Generic;
    using Controllers.Synchronization;
    using DataModel.BasicProperties;
    using DataModel.Telemetry;
    using Settings.DTO;
    using TelemetryManagement.DTO;
    using TelemetryManagement.StoryBoard;

    public class CompareToReferenceDataExtractor : ISingleSeriesDataExtractor
    {
        private readonly ITelemetryViewsSynchronization _telemetryViewsSynchronization;
        private readonly TelemetryStoryBoardFactory _telemetryStoryBoardFactory;
        public event EventHandler<EventArgs> DataRefreshRequested;

        private readonly Dictionary<string, TelemetryStoryboard> _loadedTelemetries;
        private TelemetryStoryboard _referenceLap;

        public CompareToReferenceDataExtractor(ITelemetryViewsSynchronization telemetryViewsSynchronization, TelemetryStoryBoardFactory telemetryStoryBoardFactory)
        {
            _telemetryViewsSynchronization = telemetryViewsSynchronization;
            _telemetryStoryBoardFactory = telemetryStoryBoardFactory;
            _loadedTelemetries = new Dictionary<string, TelemetryStoryboard>();
            Subscribe();
        }

        private void Subscribe()
        {
            _telemetryViewsSynchronization.LapLoaded += TelemetryViewsSynchronizationOnLapLoaded;
            _telemetryViewsSynchronization.LapUnloaded += TelemetryViewsSynchronizationOnLapUnloaded;
            _telemetryViewsSynchronization.ReferenceLapSelected += TelemetryViewsSynchronizationOnReferenceLapSelected;
        }

        private void TelemetryViewsSynchronizationOnReferenceLapSelected(object sender, LapSummaryArgs e)
        {
            _referenceLap = e.LapSummary != null ? _loadedTelemetries[e.LapSummary.Id] : null;
            DataRefreshRequested?.Invoke(this, new EventArgs());
        }

        private void TelemetryViewsSynchronizationOnLapUnloaded(object sender, LapSummaryArgs e)
        {
            _loadedTelemetries.Remove(e.LapSummary.Id);
        }

        private void TelemetryViewsSynchronizationOnLapLoaded(object sender, LapTelemetryArgs e)
        {
            _loadedTelemetries.Add(e.LapTelemetry.LapSummary.Id, _telemetryStoryBoardFactory.Create(e.LapTelemetry));
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