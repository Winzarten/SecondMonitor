namespace SecondMonitor.Rating.Application.Controller.RaceObserver.States.Context
{
    using System.Collections.Generic;
    using DataModel.Summary;

    public class QualificationContext
    {
        public int QualificationDifficulty { get; set; }

        public List<Driver> LastQualificationResult { get; set; }
    }
}