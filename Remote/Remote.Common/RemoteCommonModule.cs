namespace SecondMonitor.Remote.Common
{
    using Adapter;
    using Comparators;
    using Ninject.Modules;

    public class RemoteCommonModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDatagramPayloadUnPacker>().To<DatagramPayloadUnPacker>();
            Bind<IDatagramPayloadPacker>().To<DatagramPayloadPacker>();
            Bind<ISimulatorSourceInfoComparator>().To<SimulatorSourceInfoComparator>();
        }
    }
}