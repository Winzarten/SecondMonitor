namespace SecondMonitor.PluginManager.GameConnector
{
    using System;

    public class MessageArgs : EventArgs
    {

        public MessageArgs(string message)
        {
            Message = message;
        }

        public string Message
        {
            get;
        }
    }
}