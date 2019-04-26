namespace SecondMonitor.Rating.Common
{
    using Ninject.Modules;
    using Repository;

    public class RatingCommonModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IRatingRepository>().To<RatingRepository>();
        }
    }
}