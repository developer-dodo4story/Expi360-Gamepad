using EnhancedDodoServer;

namespace EnhancedDodoServer
{
    /// <summary>
    /// Interface of a udp client
    /// </summary>
    public interface IClientUDP
    {
        void ConnectUDP();
        void SendToServerUDP(string message);
    }
}