namespace SecondMonitor.PluginManager.GameConnector
{
    using System;

    public interface IGameConnector
    {
        event EventHandler<DataEventArgs> DataLoaded;

        event EventHandler<EventArgs> ConnectedEvent;

        event EventHandler<EventArgs> Disconnected;

        event EventHandler<DataEventArgs> SessionStarted;

        event EventHandler<MessageArgs> DisplayMessage;

        bool IsConnected
        {
            get;
        }

        int TickTime
        {
            get;
            set;
        }

        bool TryConnect();

        void ASyncConnect();

    }
}
