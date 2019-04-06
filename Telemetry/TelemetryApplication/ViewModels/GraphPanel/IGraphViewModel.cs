namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;
    using Controllers.Synchronization;
    using Controllers.Synchronization.Graphs;
    using DataModel.BasicProperties;
    using DataModel.Telemetry;
    using OxyPlot;
    using SecondMonitor.ViewModels;
    using Settings.DTO;
    using TelemetryManagement.DTO;

    public interface IGraphViewModel : IViewModel, IDisposable
    {
        string Title { get; }
        bool HasValidData { get; set; }
        PlotModel PlotModel { get; }
        XAxisKind XAxisKind { get; set; }
        Dictionary<string, (double x, Color color)> SelectedDistances { get; set; }
        ILapColorSynchronization LapColorSynchronization { get; set; }
        IGraphViewSynchronization GraphViewSynchronization { get; set; }
        DistanceUnits DistanceUnits { get; set; }
        DistanceUnits SuspensionDistanceUnits { get; set; }
        VelocityUnits VelocityUnits { get; set; }
        VelocityUnits VelocityUnitsSmall { get; set; }
        TemperatureUnits TemperatureUnits { get; set; }
        PressureUnits PressureUnits { get; set; }
        AngleUnits AngleUnits { get; set; }
        ForceUnits ForceUnits { get; set; }

        void AddLapTelemetry(LapTelemetryDto lapTelemetryDto);
        void RemoveLapTelemetry(LapSummaryDto lapSummaryDto);
        void UpdateXSelection(string lapId, TimedTelemetrySnapshot timedTelemetrySnapshot);
    }
}