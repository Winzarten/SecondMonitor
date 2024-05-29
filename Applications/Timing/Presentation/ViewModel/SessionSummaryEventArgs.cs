namespace SecondMonitor.Timing.Presentation.ViewModel
{
    using System;
    using DataModel.Summary;

    public class SessionSummaryEventArgs : EventArgs
    {
        public SessionSummaryEventArgs(SessionSummary summary)
        {
            Summary = summary;
        }
        public SessionSummary Summary { get; }
    }
}