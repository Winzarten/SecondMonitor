namespace SecondMonitor.Rating.Application
{
    using Controller;
    using Ninject.Modules;
    using ViewModels;
    using ViewModels.Rating;

    public class RatingApplicationModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IRatingApplicationController>().To<RatingApplicationController>();
            Bind<IRatingApplicationViewModel>().To<RatingApplicationViewModel>();
            Bind<IRatingViewModel>().To<RatingViewModel>();
        }
    }
}