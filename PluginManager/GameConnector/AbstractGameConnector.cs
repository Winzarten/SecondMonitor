namespace SecondMonitor.PluginManager.GameConnector
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    using SecondMonitor.DataModel.Snapshot;

    public abstract class AbstractGameConnector : IGameConnector
    {
        public event EventHandler<DataEventArgs> DataLoaded;

        public event EventHandler<EventArgs> ConnectedEvent;

        public event EventHandler<EventArgs> Disconnected;

        public event EventHandler<DataEventArgs> SessionStarted;

        public event EventHandler<MessageArgs> DisplayMessage;

        public abstract bool IsConnected { get; }

        public int TickTime { get; set; }

        private readonly string[] executables;

        private readonly Queue<SimulatorDataSet> _queue = new Queue<SimulatorDataSet>();

        private Thread _daemonThread;

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

        public void ASyncConnect()
        {
            Thread asyncConnectThread = new Thread(AsyncConnector) { IsBackground = true };
            asyncConnectThread.Start();
        }

        public bool TryConnect()
        {
            return Connect();
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
                StartDaemon();
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }


        protected abstract void OnConnection();

        protected abstract void ResetConnector();

        protected abstract void DaemonMethod();

        protected void AddToQueue(SimulatorDataSet set)
        {
            lock (_queue)
            {
                _queue.Enqueue(set);
            }
        }

        private void AsyncConnector()
        {
            while (!TryConnect())
            {
                Thread.Sleep(10);
            }
        }

        private void StartDaemon()
        {
            if (_daemonThread != null && _daemonThread.IsAlive)
            {
                throw new InvalidOperationException("Daemon is already running");
            }

            ResetConnector();
            ShouldDisconnect = false;
            _queue.Clear();
            _daemonThread = new Thread(DaemonMethod) { IsBackground = true };
            _daemonThread.Start();

            Thread queueProcessorThread = new Thread(QueueProcessor) { IsBackground = true };
            queueProcessorThread.Start();

        }

        private void QueueProcessor()
        {
            while (ShouldDisconnect == false)
            {
                SimulatorDataSet set;
                while (_queue.Count != 0)
                {
                    lock (_queue)
                    {
                        set = _queue.Dequeue();
                    }

                    RaiseDataLoadedEvent(set);
                }

                Thread.Sleep(TickTime);
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