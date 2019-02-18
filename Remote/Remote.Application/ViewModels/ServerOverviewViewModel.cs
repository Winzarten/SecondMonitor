namespace SecondMonitor.Remote.Application.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using LiteNetLib;
    using SecondMonitor.ViewModels;
    using SecondMonitor.ViewModels.Factory;

    public class ServerOverviewViewModel : AbstractViewModel, IServerOverviewViewModel
    {
        private readonly IViewModelFactory _viewModelFactory;
        private List<string> _availableIps;
        private bool _isRunning;
        private ObservableCollection<IClientViewModel> _connectedClients;
        private readonly Dictionary<string, IClientViewModel> _ipClientDictionary;
        private double _throttleInput;
        private double _clutchInput;
        private double _brakeInput;
        private string _connectedSimulator;

        public ServerOverviewViewModel(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
            _ipClientDictionary = new Dictionary<string, IClientViewModel>();
            ConnectedClients = new ObservableCollection<IClientViewModel>();
        }

        public List<string> AvailableIps
        {
            get => _availableIps;
            set => SetProperty(ref _availableIps, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public double ThrottleInput
        {
            get => _throttleInput;
            set => SetProperty(ref _throttleInput, value);
        }

        public double ClutchInput
        {
            get => _clutchInput;
            set => SetProperty(ref _clutchInput, value);
        }

        public double BrakeInput
        {
            get => _brakeInput;
            set => SetProperty(ref _brakeInput, value);
        }

        public string ConnectedSimulator
        {
            get => _connectedSimulator;
            set => SetProperty(ref _connectedSimulator, value);
        }

        public ObservableCollection<IClientViewModel> ConnectedClients
        {
            get => _connectedClients;
            private set => SetProperty(ref _connectedClients, value);
        }

        public void AddClient(NetPeer netPeer)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => AddClient(netPeer));
                return;
            }
            RemoveClient(netPeer);
            IClientViewModel newViewModel = _viewModelFactory.Create<IClientViewModel>();
            newViewModel.FromModel(netPeer);
            _connectedClients.Add(newViewModel);
            _ipClientDictionary[netPeer.EndPoint.Host] = newViewModel;
        }

        public void RemoveClient(NetPeer netPeer)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => RemoveClient(netPeer));
                return;
            }

            if (!_ipClientDictionary.ContainsKey(netPeer.EndPoint.Host))
            {
                return;
            }

            IClientViewModel removedViewModel = _ipClientDictionary[netPeer.EndPoint.Host];
            _ipClientDictionary.Remove(netPeer.EndPoint.Host);
            _connectedClients.Remove(removedViewModel);
        }
    }
}