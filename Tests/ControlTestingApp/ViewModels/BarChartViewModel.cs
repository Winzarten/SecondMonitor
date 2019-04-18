namespace ControlTestingApp.ViewModels
{
    using System;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;

    public class BarChartViewModel
    {
        private ColumnSeries _s1;

        public BarChartViewModel()
        {
            CreatePlotModel();
        }

        public PlotModel PlotModel { get; private set; }

        private void CreatePlotModel()
        {
            var model = new PlotModel
            {
                Title = "BarSeries",
                IsLegendVisible = false,
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.BottomCenter,
                LegendOrientation = LegendOrientation.Horizontal,
                LegendBorderThickness = 0,
                TextColor = OxyColors.White,
                PlotAreaBorderColor = OxyColors.White,
            };

            _s1 = new ColumnSeries() {Title = "Series 1", StrokeColor = OxyColors.White, StrokeThickness = 1, LabelPlacement = LabelPlacement.Inside, LabelFormatString = "{0:.00}%"};
            _s1.Items.Add(new ColumnItem() {Value = 25});
            _s1.Items.Add(new ColumnItem {Value = 137});
            _s1.Items.Add(new ColumnItem {Value = 18});
            _s1.Items.Add(new ColumnItem {Value = 40});

            _s1.Selectable = true;
            _s1.SelectionMode = SelectionMode.Multiple;


            var categoryAxis = new CategoryAxis {Position = AxisPosition.Bottom, GapWidth = 0, MajorGridlineThickness = 1, MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = OxyColors.White, TicklineColor = OxyColors.White};
            categoryAxis.Labels.Add("Category A");
            categoryAxis.Labels.Add("Category B");
            categoryAxis.Labels.Add("Category C");
            categoryAxis.Labels.Add("Category D");
            categoryAxis.Selectable = true;
            categoryAxis.SelectionMode = SelectionMode.Multiple;


            var valueAxis = new LinearAxis {Position = AxisPosition.Left, MinimumPadding = 0, MaximumPadding = 0.0, AbsoluteMinimum = 0, MajorGridlineThickness = 1, MajorGridlineStyle  = LineStyle.Solid, MajorGridlineColor = OxyColors.White, MajorStep = 10, AxislineColor = OxyColors.White, TicklineColor = OxyColors.White };
            model.Series.Add(_s1);
            model.Axes.Add(categoryAxis);
            model.Axes.Add(valueAxis);
            model.SelectionColor = OxyColors.Red;

            PlotModel = model;
        }
    }
}