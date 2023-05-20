using EnhancedDodoServer;

namespace EnhancedDodoServer
{
    /// <summary>
    /// Interface of a tcp client
    /// </summary>
    public interface IClientTCP
    {
        void ConnectTCP();
        void SendToServerTCP(string message);
    }
}