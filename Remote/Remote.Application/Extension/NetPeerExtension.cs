using LiteNetLib;

namespace SecondMonitor.Remote.Application.Extension
{
    public static class NetPeerExtension
    {
        public static string GetIdentifier(this NetPeer netPeer)
        {
            return $"Host:{netPeer.EndPoint.Host}:{netPeer.EndPoint.Port}";
        }
    }
}