namespace SecondMonitor.PluginManager.GameConnector
{
    using System;

    public class MessageArgs : EventArgs
    {

        public MessageArgs(string message, Action action)
        {
            Action = action;
            Message = message;
        }

        public MessageArgs(string message)
            : this(message, null)
        {
        }

        public string Message
        {
            get;
        }

        public bool IsDecision => Action != null;

        public Action Action { get; }
    }
}