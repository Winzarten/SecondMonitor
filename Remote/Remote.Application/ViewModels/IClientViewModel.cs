namespace SecondMonitor.Remote.Application.ViewModels
{
    using LiteNetLib;
    using SecondMonitor.ViewModels;

    public interface IClientViewModel : IViewModel<NetPeer>
    {
        string IpAddress { get; }
        int Port { get; }
    }
}