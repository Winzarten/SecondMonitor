﻿namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.AggregatedCharts
{
    using System.Collections.Generic;

    public class CompositeAggregatedChartsViewModel : AggregatedChartViewModel
    {
        private IAggregatedChartViewModel _mainAggregatedChartViewModel;
        private readonly List<IAggregatedChartViewModel> _childAggregatedChartViewModels;

        public CompositeAggregatedChartsViewModel()
        {
            _childAggregatedChartViewModels = new List<IAggregatedChartViewModel>();
        }

        public IAggregatedChartViewModel MainAggregatedChartViewModel
        {
            get => _mainAggregatedChartViewModel;
            set => SetProperty(ref _mainAggregatedChartViewModel, value);
        }

        public IReadOnlyCollection<IAggregatedChartViewModel> ChildAggregatedChartViewModels => _childAggregatedChartViewModels;

        public void AddChildAggregatedChildViewModel(IAggregatedChartViewModel aggregatedChartViewModel)
        {
            _childAggregatedChartViewModels.Add(aggregatedChartViewModel);
        }

    }
}