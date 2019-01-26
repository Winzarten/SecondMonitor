namespace SecondMonitor.Telemetry.TelemetryApplication.ViewModels.SettingsWindow
{
    using SecondMonitor.ViewModels;
    using Settings.DTO;

    public class GraphSettingsViewModel : AbstractViewModel<GraphSettingsDto>, IGraphSettingsViewModel
    {
        private string _title;
        private int _priority;
        private GraphLocationKind _graphLocation;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public int Priority
        {
            get => _priority;
            set => SetProperty(ref _priority, value);
        }

        public GraphLocationKind GraphLocation
        {
            get => _graphLocation;
            set => SetProperty(ref _graphLocation, value);
        }

        protected override void ApplyModel(GraphSettingsDto model)
        {
            Title = model.Title;
            Priority = model.GraphPriority;
            GraphLocation = model.GraphLocation;
        }

        public override GraphSettingsDto SaveToNewModel()
        {
            return new GraphSettingsDto()
            {
                GraphLocation = GraphLocation,
                GraphPriority = Priority,
                Title = Title
            };
        }
    }
}