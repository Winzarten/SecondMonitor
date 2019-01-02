namespace SecondMonitor.Telemetry.TelemetryApplication.Controllers.MainWindow.GraphPanel
{
    using System.Collections.Generic;
    using ViewModels.GraphPanel;

    public interface IGraphViewModelsProvider
    {
        IEnumerable<IGraphViewModel> GetLeftSideViewModels();
        IEnumerable<IGraphViewModel> GetRightSideViewModels();
    }
}