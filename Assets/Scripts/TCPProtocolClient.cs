using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using EnhancedDodoServer;

namespace EnhancedDodoServer
{
    /// <summary>
    /// Represents a tcp client on the client side
    /// </summary>
    public class TCPProtocolClient : Singleton<TCPProtocolClient>
    {
        /// <summary>
        /// Handles opening and closing sockets
        /// </summary>
        TcpClient tcpClient;
        /// <summary>
        /// Holds references to the client socket
        /// </summary>
        TCPConnectedClient tcpConnectedClient;
        /// <summary>
        /// Opens a new socket
        /// </summary>
        /// <param name="_serverIP"></param>
        public void Connect(IPAddress _serverIP)
        {
            tcpClient = new TcpClient();
            tcpConnectedClient = new TCPConnectedClient(tcpClient);
            tcpConnectedClient.BeginConnect();
            tcpConnectedClient.onRead += Read;
        }
        /// <summary>
        /// Closes socket
        /// </summary>
        public void Disconnect()
        {
            tcpConnectedClient.Disconnect();
        }
        /// <summary>
        /// When a new tcp message arrives
        /// </summary>
        /// <param name="message"></param>
        public void Read(string message)
        {
            Client.instance.ReadTCP(message);
        }
        /// <summary>
        /// Sends a message over tcp
        /// </summary>
        /// <param name="message">The message</param>
        public void Send(string message)
        {
            tcpConnectedClient.Send(message);
        }
    }
}