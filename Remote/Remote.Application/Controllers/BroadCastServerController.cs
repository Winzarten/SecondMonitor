﻿using SecondMonitor.DataModel.BasicProperties;
using SecondMonitor.Remote.Common.Adapter;

namespace SecondMonitor.Remote.Application.Controllers
{
    using System;
    using System.Diagnostics;
    using System.IO;
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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IPluginSettingsProvider _pluginSettingsProvider;
        private readonly IServerOverviewViewModel _serverOverviewViewModel;
        private readonly IDatagramPayloadAdapter _datagramPayloadAdapter;
        private EventBasedNetListener _eventBasedNetListener;
        private NetManager _server;
        private Task _checkLoop;
        private CancellationTokenSource _serverCheckLoopSource;
        private Stopwatch _lastPackedStopWatch;
        private readonly IFormatter _formatter;
        private DatagramPayload _lastDatagramPayload;

        public BroadCastServerController(IPluginSettingsProvider pluginSettingsProvider, IServerOverviewViewModel serverOverviewViewModel, IDatagramPayloadAdapter datagramPayloadAdapter)
        {
            _pluginSettingsProvider = pluginSettingsProvider;
            _serverOverviewViewModel = serverOverviewViewModel;
            _datagramPayloadAdapter = datagramPayloadAdapter;
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
            DatagramPayload lastPayload = _lastDatagramPayload;

            if (lastPayload != null && lastPayload.PayloadKind != DatagramPayloadKind.HearthBeat)
            {
                DatagramPayload initialPayload = new DatagramPayload() {Payload = lastPayload.Payload, PayloadKind = DatagramPayloadKind.SessionStart};
                SendPackage(initialPayload, peer);
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
            _lastDatagramPayload = payload;
            UpdateViewModelInputs(payload.InputInfo);
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
            _serverOverviewViewModel.ThrottleInput = inputInfo.ThrottlePedalPosition * 100;
            _serverOverviewViewModel.ClutchInput = inputInfo.ClutchPedalPosition * 100;
            _serverOverviewViewModel.BrakeInput = inputInfo.BrakePedalPosition * 100;
            _serverOverviewViewModel.ConnectedSimulator = source == "Remote" ? "None" : source;
        }

        private void SendKeepAlivePacket()
        {
            Random random = new Random();
            SimulatorDataSet dataSet = new SimulatorDataSet("Remote") {InputInfo = {BrakePedalPosition = random.NextDouble(), ThrottlePedalPosition = random.NextDouble(), ClutchPedalPosition = random.NextDouble()}};
            DatagramPayload payload = new DatagramPayload {Payload = dataSet, PayloadKind = DatagramPayloadKind.HearthBeat};
            SendPackage(payload);
        }

        public void SendSessionStartedPackage(SimulatorDataSet simulatorDataSet)
        {
            _lastPackedStopWatch.Restart();
            DatagramPayload payload = new DatagramPayload() {Payload = simulatorDataSet, PayloadKind = DatagramPayloadKind.SessionStart};
            SendPackage(payload);
        }

        public void SendRegularDataPackage(SimulatorDataSet simulatorDataSet)
        {
            _lastPackedStopWatch.Restart();
            DatagramPayload payload = new DatagramPayload() {Payload = simulatorDataSet, PayloadKind = DatagramPayloadKind.Normal};
            SendPackage(payload);
        }
    }
}