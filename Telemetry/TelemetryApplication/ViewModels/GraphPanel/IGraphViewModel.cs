namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.GraphPanel
{
    using System;
    using Controllers.Synchronization;
    using Controllers.Synchronization.Graphs;
    using DataModel.BasicProperties;
    using OxyPlot;
    using SecondMonitor.ViewModels;
    using TelemetryManagement.DTO;

    public interface IGraphViewModel : IViewModel, IDisposable
    {
        string Title { get; }
        bool HasValidData { get; set; }
        PlotModel PlotModel { get; }
        Distance SelectedDistance { get; set; }
        ILapColorSynchronization LapColorSynchronization { get; set; }
        IGraphViewSynchronization GraphViewSynchronization { get; set; }
        Distance TrackDistance { get; set; }
        DistanceUnits DistanceUnits { get; set; }
        DistanceUnits SuspensionDistanceUnits { get; set; }
        VelocityUnits VelocityUnits { get; set; }
        TemperatureUnits TemperatureUnits { get; set; }
        PressureUnits PressureUnits { get; set; }

        void AddLapTelemetry(LapTelemetryDto lapTelemetryDto);
        void RemoveLapTelemetry(LapSummaryDto lapSummaryDto);
    }
}