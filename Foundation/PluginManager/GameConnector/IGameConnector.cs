namespace SecondMonitor.PluginManager.GameConnector
{
    using System;
    using System.Threading.Tasks;

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

        bool TryConnect();

        Task FinnishConnectorAsync();

        void StartConnectorLoop();

    }
}
