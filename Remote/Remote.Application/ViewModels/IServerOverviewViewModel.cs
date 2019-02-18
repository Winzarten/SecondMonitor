namespace SecondMonitor.Remote.Application.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using LiteNetLib;
    using SecondMonitor.ViewModels;

    public interface IServerOverviewViewModel : IViewModel
    {
        List<string> AvailableIps { get; set; }

        bool IsRunning { get; set; }
        double ThrottleInput { get; set; }
        double ClutchInput { get; set; }
        double BrakeInput { get; set; }
        string ConnectedSimulator { get; set; }

        ObservableCollection<IClientViewModel> ConnectedClients { get; }

        void AddClient(NetPeer netPeer);
        void RemoveClient(NetPeer netPeer);
    }
}