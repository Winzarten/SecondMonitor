namespace SecondMonitor.Rating.Application.Controller.RaceObserver
{
    using System;
    using DataModel;
    using SimulatorRating;
    using States;
    using States.Context;

    public class RaceStateFactory : IRaceStateFactory
    {
        public IRaceState CreateInitialState(ISimulatorRatingController simulatorRatingController)
        {
            if (SimulatorsNameMap.IsR3ESimulator(simulatorRatingController.SimulatorName))
            {
                return new IdleState(new SharedContext() {SimulatorRatingController = simulatorRatingController});
            }

            throw new ArgumentException($"Simulator {simulatorRatingController.SimulatorName} is not supported");
        }
    }
}