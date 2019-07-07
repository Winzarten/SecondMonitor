namespace SecondMonitor.Rating.Application
{
    using Controller;
    using Controller.RaceObserver;
    using Controller.SimulatorRating;
    using Ninject.Modules;
    using ViewModels;
    using ViewModels.Rating;

    public class RatingApplicationModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IRatingApplicationController>().To<RatingApplicationController>();
            Bind<IRaceObserverController>().To<RaceObserverController>();
            Bind<IRatingStorageController>().To<RatingStorageController>();
            Bind<ISimulatorRatingControllerFactory>().To<SimulatorRatingControllerFactory>();
            Bind<ISimulatorRatingController>().To<SimulatorRatingController>();
            Bind<IRaceStateFactory>().To<RaceStateFactory>();

            Bind<IRatingApplicationViewModel>().To<RatingApplicationViewModel>();
            Bind<IRatingViewModel>().To<RatingViewModel>();
        }
    }
}