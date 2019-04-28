namespace SecondMonitor.Rating.Application.Controller.RaceObserver
{
    using System;
    using DataModel;
    using States;

    public class RaceStateFactory : IRaceStateFactory
    {
        public IRaceState CreateInitialState(string simulatorName)
        {
            if (SimulatorsNameMap.IsR3ESimulator(simulatorName))
            {
                return new IdleState();
            }

            throw new ArgumentException($"Simulator {simulatorName} is not supported");
        }
    }
}