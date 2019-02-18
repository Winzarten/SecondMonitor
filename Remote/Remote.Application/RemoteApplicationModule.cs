namespace SecondMonitor.Remote.Application
{
    using Controllers;
    using Ninject.Modules;
    using ViewModels;

    public class RemoteApplicationModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IBroadCastServerController>().To<BroadCastServerController>();
            Bind<IServerOverviewViewModel>().To<ServerOverviewViewModel>().InSingletonScope();
            Bind<IClientViewModel>().To<ClientViewModel>();
        }
    }
}