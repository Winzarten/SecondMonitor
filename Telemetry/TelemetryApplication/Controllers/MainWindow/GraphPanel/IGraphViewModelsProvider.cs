namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.GraphPanel
{
    using System.Collections.Generic;
    using ViewModels.GraphPanel;

    public interface IGraphViewModelsProvider
    {
        IEnumerable<(IGraphViewModel graphViewModel, int priority)> GetLeftSideViewModels();
        IEnumerable<(IGraphViewModel graphViewModel, int priority)> GetRightSideViewModels();
    }
}