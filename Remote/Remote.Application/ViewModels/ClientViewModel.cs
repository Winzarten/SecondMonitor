namespace SecondMonitor.Remote.Application.ViewModels
{
    using LiteNetLib;
    using SecondMonitor.ViewModels;

    public class ClientViewModel : AbstractViewModel<NetPeer>, IClientViewModel
    {
        private string _ipAddress;

        public string IpAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress, value);
        }

        protected override void ApplyModel(NetPeer model)
        {
            IpAddress = model.EndPoint.Host;
        }

        public override NetPeer SaveToNewModel()
        {
            throw new System.NotImplementedException();
        }
    }
}