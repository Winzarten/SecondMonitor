namespace SecondMonitor.Remote.Application.ViewModels
{
    using LiteNetLib;
    using SecondMonitor.ViewModels;

    public class ClientViewModel : AbstractViewModel<NetPeer>, IClientViewModel
    {
        private string _ipAddress;
        private int _port;

        public string IpAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress, value);
        }

        public int Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        protected override void ApplyModel(NetPeer model)
        {
            IpAddress = model.EndPoint.Host;
            Port = model.EndPoint.Port;
        }

        public override NetPeer SaveToNewModel()
        {
            throw new System.NotImplementedException();
        }
    }
}