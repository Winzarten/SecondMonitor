namespace SecondMonitor.Remote.Connector
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Model;
    using Contracts.NInject;
    using LiteNetLib;
    using LiteNetLib.Utils;
    using NLog;
    using PluginManager.GameConnector;
    using PluginsConfiguration.Common.Controller;
    using PluginsConfiguration.Common.DataModel;

    public class RemoteConnector : IGameConnector
    {
        public event EventHandler<DataEventArgs> DataLoaded;
        public event EventHandler<EventArgs> ConnectedEvent;
        public event EventHandler<EventArgs> Disconnected;
        public event EventHandler<DataEventArgs> SessionStarted;
        public event EventHandler<MessageArgs> DisplayMessage;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Lazy<RemoteConfiguration> _remoteConfiguration;
        private Task _clientCheckLoopTask;

        private readonly EventBasedNetListener _listener;
        private readonly NetManager _client;
        private NetEndPoint _serverEndPoint;
        private NetPeer _serverPeer;
        private readonly IFormatter _formatter;

        public RemoteConnector()
        {
            _formatter = new BinaryFormatter();
            _listener = new EventBasedNetListener();
            _client = new NetManager(_listener, DatagramPayload.Version) { DiscoveryEnabled = true }; ;
            KernelWrapper kernelWrapper = new KernelWrapper();
            _remoteConfiguration = new Lazy<RemoteConfiguration>(() => kernelWrapper.Get<IPluginSettingsProvider>().RemoteConfiguration);
        }

        private RemoteConfiguration RemoteConfiguration => _remoteConfiguration.Value;

        public bool IsConnected { get; private set; }

        public bool TryConnect()
        {
            if (!RemoteConfiguration.IsRemoteConnectorEnabled)
            {
                return false;
            }

            if (_serverPeer?.ConnectionState == ConnectionState.Connected)
            {
                Logger.Info($"Connected to {_serverPeer.EndPoint.Host}:{_serverPeer.EndPoint.Host}");
                return true;
            }

            if (_clientCheckLoopTask == null)
            {
                StartConnector();
            }

            if (RemoteConfiguration.IsFindInLanEnabled)
            {
                TryConnectUsingDiscovery();
            }
            else
            {
                _serverEndPoint = new NetEndPoint(RemoteConfiguration.IpAddress, RemoteConfiguration.Port);
                ConnectToServer();
            }

            Thread.Sleep(200);

            if (_serverPeer?.ConnectionState != ConnectionState.Connected)
            {
                return false;
            }

            Logger.Info($"Connected to {_serverPeer.EndPoint.Host}:{_serverPeer.EndPoint.Host}");
            return true;

        }

        private void TryConnectUsingDiscovery()
        {
            if (_serverEndPoint != null)
            {
                ConnectToServer();
                return;
            }
            Logger.Info($"Sending Discovery Request: {DatagramPayload.Version}");

            NetDataWriter netDataWriter = new NetDataWriter();
            netDataWriter.Put(DatagramPayload.Version);
            _client.SendDiscoveryRequest(netDataWriter, RemoteConfiguration.Port);
            netDataWriter.Reset();
        }

        private void ConnectToServer()
        {
            if (_serverEndPoint == null || _serverPeer?.ConnectionState == ConnectionState.InProgress)
            {
                return;
            }

            Logger.Info($"Trying to Connecto to Server {_serverEndPoint.Host}:{_serverEndPoint.Port}");
            _serverPeer = _client.Connect(_serverEndPoint);
        }

        public Task FinnishConnectorAsync()
        {
            return Task.CompletedTask;
        }

        public void StartConnectorLoop()
        {
            IsConnected = true;
        }

        private void StartConnector()
        {
            _client.Start();
            SubscribeEventBasedListener();
            _clientCheckLoopTask = ClientCheckLoop();
        }

        private void SubscribeEventBasedListener()
        {
            if (_listener == null)
            {
                return;
            }

            _listener.PeerConnectedEvent += EventBasedNetListenerOnPeerConnectedEvent;
            _listener.PeerDisconnectedEvent += EventBasedNetListenerOnPeerDisconnectedEvent;
            _listener.NetworkReceiveEvent += EventBasedNetListenerOnNetworkReceiveEvent;
            _listener.NetworkReceiveUnconnectedEvent += EventBasedNetListenerOnNetworkReceiveUnconnectedEvent;
        }

        private void EventBasedNetListenerOnNetworkReceiveUnconnectedEvent(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType)
        {
            if (messageType != UnconnectedMessageType.DiscoveryResponse)
            {
                return;
            }

            string discoveryMessage = reader.GetString();

            Logger.Info($"Discovery ResponseFrom from {remoteEndPoint.Host}:{remoteEndPoint.Port}, Message:{discoveryMessage}");
            if (discoveryMessage != DatagramPayload.Version)
            {
                Logger.Info("Version Do not Match - Ignoring");
                return;
            }

            _serverEndPoint = remoteEndPoint;
            Logger.Info("Version Do Match - Will be used as server");
        }

        private void EventBasedNetListenerOnNetworkReceiveEvent(NetPeer peer, NetDataReader reader)
        {
            //Not the used connector = end
            if (!IsConnected)
            {
                return;
            }

            DatagramPayload payload;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(reader.Data, 0, reader.AvailableBytes);
                memoryStream.Seek(0, SeekOrigin.Begin);
                payload =  (DatagramPayload) _formatter.Deserialize(memoryStream);
            }

            if (payload.PayloadKind == DatagramPayloadKind.SessionStart)
            {
                SessionStarted?.Invoke(this, new DataEventArgs(payload.Payload));
            }
            else
            {
                DataLoaded?.Invoke(this, new DataEventArgs(payload.Payload));
            }
        }

        private void EventBasedNetListenerOnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectinfo)
        {
            if (disconnectinfo.Reason == DisconnectReason.ConnectionFailed)
            {
                return;
            }

            Logger.Info($"Server Disconnected - {disconnectinfo.Reason}");
            Disconnected?.Invoke(this, new EventArgs());
        }

        private void EventBasedNetListenerOnPeerConnectedEvent(NetPeer peer)
        {

        }

        private async Task ClientCheckLoop()
        {
            while (true)
            {
                await Task.Delay(5);
                _client.PollEvents();
            }
        }
    }
}