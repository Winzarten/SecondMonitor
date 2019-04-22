﻿namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.AggregatedCharts
{
    public class SplitAggregatedChartViewModel : AbstractAggregatedChartViewModel
    {
        public IAggregatedChartViewModel TopViewModel { get; set; }
        public IAggregatedChartViewModel BottomViewModel { get; set; }
    }
}