﻿namespace SecondMonitor.Rating.Common
{
    using Configuration;
    using Ninject.Modules;
    using Repository;

    public class RatingCommonModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IRatingRepository>().To<RatingRepository>();
            Bind<ISimulatorRatingConfigurationProvider>().To<SimulatorRatingConfigurationProvider>();
        }
    }
}