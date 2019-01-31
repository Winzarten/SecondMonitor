namespace SecondMonitor.PluginManager.GameConnector
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using DataModel.Snapshot;
    using NLog;

    public abstract class AbstractGameConnector : IGameConnector
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public event EventHandler<DataEventArgs> DataLoaded;

        public event EventHandler<EventArgs> ConnectedEvent;

        public event EventHandler<EventArgs> Disconnected;

        public event EventHandler<DataEventArgs> SessionStarted;

        public event EventHandler<MessageArgs> DisplayMessage;

        public abstract bool IsConnected { get; }

        public int TickTime { get; set; }

        private readonly string[] executables;

        private readonly Queue<SimulatorDataSet> _queue = new Queue<SimulatorDataSet>();

        private Task _daemonTask;
        private Task _queueProcessorTask;
        private CancellationTokenSource _cancellationTokenSource;

        protected Process Process { get; private set; }

        protected AbstractGameConnector(string[] executables)
        {
            this.executables = executables;
            TickTime = 10;
        }

        protected bool ShouldDisconnect
        {
            get;
            set;
        }

        protected abstract string ConnectorName { get; }

        public bool IsProcessRunning()
        {
            if (Process != null)
            {
                if (!Process.HasExited)
                {
                    return true;
                }

                Process = null;
                return false;
            }

            foreach (var processName in executables)
            {
                var processes = Process.GetProcessesByName(processName);
                if (processes.Length <= 0)
                {
                    continue;
                }

                Process = processes[0];
                return true;
            }

            return false;
        }

        public bool TryConnect()
        {
            return Connect();
        }

        public async Task FinnishConnectorAsync()
        {
            _cancellationTokenSource.Cancel();
            try
            {
                await _daemonTask;

            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in connector");
            }

            try
            {
                await _queueProcessorTask;
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error in connector");
            }
            _daemonTask = null;
            _queueProcessorTask = null;
        }

        private bool Connect()
        {
            if (!IsProcessRunning())
            {
                return false;
            }

            try
            {
                OnConnection();
                RaiseConnectedEvent();
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }


        protected abstract void OnConnection();

        protected abstract void ResetConnector();

        protected abstract Task DaemonMethod(CancellationToken cancellationToken);

        protected void AddToQueue(SimulatorDataSet set)
        {
            lock (_queue)
            {
                _queue.Enqueue(set);
            }
        }

        public void StartConnectorLoop()
        {
            if (_daemonTask != null)
            {
                throw new InvalidOperationException("Daemon is already running");
            }

            _cancellationTokenSource = new CancellationTokenSource();
            ResetConnector();
            ShouldDisconnect = false;
            _queue.Clear();
            _daemonTask = DaemonMethod(_cancellationTokenSource.Token);

            _queueProcessorTask = QueueProcessor(_cancellationTokenSource.Token);

        }

        private async Task QueueProcessor(CancellationToken cancellationToken)
        {
            while (ShouldDisconnect == false)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                lock (_queue)
                {
                    while (_queue.Count != 0)
                    {
                        SimulatorDataSet set;
                        lock (_queue)
                        {
                            set = _queue.Dequeue();
                        }

                        RaiseDataLoadedEvent(set);
                    }
                }

                await Task.Delay(TickTime, cancellationToken).ConfigureAwait(false);
            }

            lock (_queue)
            {
                _queue.Clear();
            }
        }

        protected void SendMessageToClients(string message)
        {
            SendMessageToClients(message, null);
        }

        protected void SendMessageToClients(string message, Action action)
        {
            MessageArgs args = new MessageArgs(message, action);
            DisplayMessage?.Invoke(this, args);
        }

        protected void RaiseDataLoadedEvent(SimulatorDataSet data)
        {
            DataEventArgs args = new DataEventArgs(data);
            DataLoaded?.Invoke(this, args);
        }

        protected void RaiseConnectedEvent()
        {
            EventArgs args = new EventArgs();
            ConnectedEvent?.Invoke(this, args);
        }

        protected void RaiseDisconnectedEvent()
        {
            EventArgs args = new EventArgs();
            Disconnected?.Invoke(this, args);
        }

        protected void RaiseSessionStartedEvent(SimulatorDataSet data)
        {
            DataEventArgs args = new DataEventArgs(data);
            EventHandler<DataEventArgs> handler = SessionStarted;
            handler?.Invoke(this, args);
        }
    }
}