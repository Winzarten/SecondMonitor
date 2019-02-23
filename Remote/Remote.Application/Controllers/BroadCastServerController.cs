using SecondMonitor.DataModel.BasicProperties;
using SecondMonitor.Remote.Common.Adapter;

namespace SecondMonitor.Remote.Application.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Model;
    using DataModel.Snapshot;
    using LiteNetLib;
    using LiteNetLib.Utils;
    using NLog;
    using PluginsConfiguration.Common.Controller;
    using ViewModels;

    public class BroadCastServerController : IBroadCastServerController
    {
        private object _queueLock = new object();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IPluginSettingsProvider _pluginSettingsProvider;
        private readonly IServerOverviewViewModel _serverOverviewViewModel;
        private readonly IDatagramPayloadPacker _datagramPayloadPacker;
        private EventBasedNetListener _eventBasedNetListener;
        private NetManager _server;
        private Task _checkLoop;
        private CancellationTokenSource _serverCheckLoopSource;
        private Stopwatch _lastPackedStopWatch;
        private readonly IFormatter _formatter;
        private readonly Queue<NetPeer> _newPeers;

        public BroadCastServerController(IPluginSettingsProvider pluginSettingsProvider, IServerOverviewViewModel serverOverviewViewModel, IDatagramPayloadPacker datagramPayloadPacker)
        {
            _newPeers = new Queue<NetPeer>();
            _pluginSettingsProvider = pluginSettingsProvider;
            _serverOverviewViewModel = serverOverviewViewModel;
            _datagramPayloadPacker = datagramPayloadPacker;
            _formatter = new BinaryFormatter();
        }

        public Task StartControllerAsync()
        {
            StartListeningServer();
            return Task.CompletedTask;
        }

        public async Task StopControllerAsync()
        {
            await StopListeningServer();
        }

        private void StartListeningServer()
        {
            _eventBasedNetListener = new EventBasedNetListener();
            _server = new NetManager(_eventBasedNetListener, 15, DatagramPayload.Version) {DiscoveryEnabled = true};

            SubscribeEventBasedListener();
            int port = _pluginSettingsProvider.RemoteConfiguration.Port;

            Logger.Info($"Starting Listening Server on port:{port}");
            if (!_server.Start(port))
            {
                Logger.Info($"Unable to start server, unknown error");
            }

            _serverOverviewViewModel.IsRunning = true;
            Logger.Info($"Server Started:{port}");

            _serverCheckLoopSource = new CancellationTokenSource();
            _checkLoop = ServerLoop(_serverCheckLoopSource.Token);
            _lastPackedStopWatch = Stopwatch.StartNew();
        }

        private async Task StopListeningServer()
        {
            Logger.Info("Stopping Listening Server");
            UnSubscribeEventBasedListener();
            _server.Stop();
            _serverCheckLoopSource.Cancel();

            try
            {
                await _checkLoop;
            }
            catch (TaskCanceledException)
            {
            }
        }

        private void SubscribeEventBasedListener()
        {
            if (_eventBasedNetListener == null)
            {
                return;
            }

            _eventBasedNetListener.PeerConnectedEvent += EventBasedNetListenerOnPeerConnectedEvent;
            _eventBasedNetListener.PeerDisconnectedEvent += EventBasedNetListenerOnPeerDisconnectedEvent;
            _eventBasedNetListener.NetworkReceiveEvent += EventBasedNetListenerOnNetworkReceiveEvent;
            _eventBasedNetListener.NetworkReceiveUnconnectedEvent += EventBasedNetListenerOnNetworkReceiveUnconnectedEvent;
            _eventBasedNetListener.NetworkErrorEvent += EventBasedNetListenerOnNetworkErrorEvent;
        }

        private void EventBasedNetListenerOnNetworkErrorEvent(NetEndPoint endpoint, int socketErrorCode)
        {
            Logger.Info($"Socket Error -  {endpoint.Host}:{endpoint.Port} - Error Code :{socketErrorCode}");
        }


        private void UnSubscribeEventBasedListener()
        {
            if (_eventBasedNetListener == null)
            {
                return;
            }

            _eventBasedNetListener.PeerConnectedEvent -= EventBasedNetListenerOnPeerConnectedEvent;
            _eventBasedNetListener.PeerDisconnectedEvent -= EventBasedNetListenerOnPeerDisconnectedEvent;
            _eventBasedNetListener.NetworkReceiveEvent -= EventBasedNetListenerOnNetworkReceiveEvent;
            _eventBasedNetListener.NetworkReceiveUnconnectedEvent -= EventBasedNetListenerOnNetworkReceiveUnconnectedEvent;
            _eventBasedNetListener.NetworkErrorEvent -= EventBasedNetListenerOnNetworkErrorEvent;
        }

        private void EventBasedNetListenerOnNetworkReceiveUnconnectedEvent(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType)
        {
            if (messageType != UnconnectedMessageType.DiscoveryRequest)
            {
                return;
            }

            string discoveryMessage = reader.GetString();
            Logger.Info($"Discovery Request from {remoteEndPoint.Host}:{remoteEndPoint.Port}, Message:{discoveryMessage}");

            if (discoveryMessage != DatagramPayload.Version)
            {
                Logger.Info("Versions do not match - ignored");
                return;
            }

            NetDataWriter response = new NetDataWriter();
            response.Put(DatagramPayload.Version);
            _server.SendDiscoveryResponse(response, remoteEndPoint);
        }

        private void EventBasedNetListenerOnNetworkReceiveEvent(NetPeer peer, NetDataReader reader)
        {
            Logger.Info($"Data:{reader.GetString()}");
        }

        private void EventBasedNetListenerOnPeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Logger.Info($"Client Disconnected: {peer.EndPoint.Host}:{peer.EndPoint.Port}");
            _serverOverviewViewModel.RemoveClient(peer);
        }

        private void EventBasedNetListenerOnPeerConnectedEvent(NetPeer peer)
        {
            Logger.Info($"New Client Connected: {peer.EndPoint.Host}:{peer.EndPoint.Port}");
            _serverOverviewViewModel.AddClient(peer);
            lock (_queueLock)
            {
                _newPeers.Enqueue(peer);
            }



        }

        private async Task ServerLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(5, token);
                _server.PollEvents();
                if (_lastPackedStopWatch.Elapsed.TotalMilliseconds > 1000)
                {
                    SendKeepAlivePacket();
                    _lastPackedStopWatch.Restart();
                }
            }
        }

        private void SendPackage(DatagramPayload payload)
        {
            UpdateViewModelInputs(payload.InputInfo, payload.Source);
            NetDataWriter package = new NetDataWriter();
            package.Put(SerializeDatagramPayload(payload));
            _server.SendToAll(package, SendOptions.ReliableOrdered);
        }

        private void SendPackage(DatagramPayload payload, NetPeer peer)
        {
            NetDataWriter package = new NetDataWriter();
            package.Put(SerializeDatagramPayload(payload));
            peer.Send(package, SendOptions.ReliableOrdered);
        }

        private byte[] SerializeDatagramPayload(DatagramPayload payload)
        {
            byte[] payloadBytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                _formatter.Serialize(memoryStream, payload);
                payloadBytes = memoryStream.ToArray();
            }

            return payloadBytes;
        }

        private void UpdateViewModelInputs(InputInfo inputInfo, string source)
        {
            if (inputInfo == null)
            {
                return;
            }
            _serverOverviewViewModel.ThrottleInput = inputInfo.ThrottlePedalPosition * 100;
            _serverOverviewViewModel.ClutchInput = inputInfo.ClutchPedalPosition * 100;
            _serverOverviewViewModel.BrakeInput = inputInfo.BrakePedalPosition * 100;
            _serverOverviewViewModel.ConnectedSimulator = source == "Remote" ? "None" : source;
        }

        private void SendKeepAlivePacket()
        {
            DatagramPayload payload = _datagramPayloadPacker.CreateHearthBeatDatagramPayload();
            SendPackage(payload);
        }

        public void SendSessionStartedPackage(SimulatorDataSet simulatorDataSet)
        {
            _lastPackedStopWatch.Restart();
            DatagramPayload payload = _datagramPayloadPacker.CreateSessionStartedPayload(simulatorDataSet);
            SendPackage(payload);
        }

        public void SendRegularDataPackage(SimulatorDataSet simulatorDataSet)
        {
            if (!_datagramPayloadPacker.IsMinimalPackageDelayPassed())
            {
                return;
            }

            lock (_queueLock)
            {
                if (_newPeers.Any())
                {
                    DatagramPayload initialPayload = _datagramPayloadPacker.CreateHandshakeDatagramPayload();
                    while (_newPeers.Count > 0)
                    {
                        SendPackage(initialPayload, _newPeers.Dequeue());
                    }
                }
            }

            _lastPackedStopWatch.Restart();
            DatagramPayload payload = _datagramPayloadPacker.CreateRegularDatagramPayload(simulatorDataSet);
            SendPackage(payload);
        }
    }
}