namespace SecondMonitor.Rating.Application.Controller.SimulatorRating
{
    using System;
    using System.Linq;
    using Ninject;
    using Ninject.Parameters;
    using Ninject.Syntax;

    public class SimulatorRatingControllerFactory : ISimulatorRatingControllerFactory
    {
        private readonly IResolutionRoot _resolutionRoot;
        private static readonly string[] SupportedSimulators = new[] {"R3E"};

        public SimulatorRatingControllerFactory(IResolutionRoot resolutionRoot)
        {
            _resolutionRoot = resolutionRoot;
        }

        public bool IsSimulatorSupported(string simulatorName)
        {
            return SupportedSimulators.Contains(simulatorName);
        }

        public ISimulatorRatingController CreateController(string simulatorName)
        {
            if (!IsSimulatorSupported(simulatorName))
            {
                throw new ArgumentException($"Simulator : {simulatorName}, is not supported");
            }

            return _resolutionRoot.Get<ISimulatorRatingController>(new ConstructorArgument("simulatorName", simulatorName, false));
        }
    }
}