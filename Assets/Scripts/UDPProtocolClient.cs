using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using System.Net.Sockets;
using EnhancedDodoServer;

namespace EnhancedDodoServer
{
    /// <summary>
    /// Low level class implementing a client using udp
    /// </summary>
    public class UDPProtocolClient : Singleton<UDPProtocolClient>//, INetworkDiscoverer
    {
        /// <summary>
        /// sprawdzanie czy wystapił error
        /// </summary>
        public static string exceptionError = "UDPProtocolClient Error";
        public static int sendCount = 0;

        /// <summary>
        /// Handles udp sockets and messages
        /// </summary>
        UdpClient udpClient;
        /// <summary>
        /// IPEndPoint of the server        
        /// </summary>
        IPEndPoint serverEndPoint;
        /// <summary>
        /// Handles broadcasting messages
        /// </summary>
        UdpClient broadcastClient;
        /// <summary>
        /// IPEndPoint of the broadcast messages
        /// </summary>
        IPEndPoint broadcastEndPoint;

        public UdpClient UdpClient { get => udpClient; }

        //public void DiscoverNetwork()
        //{
        //    broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, Consts.serverPort);
        //    broadcastClient = new UdpClient();
        //    broadcastClient.EnableBroadcast = true;
        //    broadcastClient.BeginReceive(OnNetworkDiscoveryCallback, null);
        //    StartCoroutine(RequestIP());
        //}
        //IEnumerator RequestIP()
        //{
        //    while (!receivedServerIP)
        //    {
        //        byte[] data = System.Text.Encoding.UTF8.GetBytes(Consts.Words.serverIPRequest);
        //        broadcastClient.Send(data, data.Length, broadcastEndPoint);
        //        yield return null;
        //    }
        //}
        //void OnNetworkDiscoveryCallback(IAsyncResult asyncResult)
        //{
        //    receivedServerIP = true;
        //    IPEndPoint iPEndPoint = null;
        //    byte[] data = broadcastClient.EndReceive(asyncResult, ref iPEndPoint);
        //    string message = System.Text.Encoding.UTF8.GetString(data); //no need to send ipaddress in the message, get it from ipendpoint            
        //    var ip = iPEndPoint.Address;
        //    Client.instance.OnNetworkDiscoveryComplete(ip);

        //}
        /// <summary>
        /// Opens a udp socket and starts to listen
        /// </summary>
        public void Connect()
        {
            try
            {
                exceptionError = "UDPProtocolClient Error"; //set default text
                Client.debugMsg += " On Connect";
                //IPAddress iPAddress = IPAddress.Any;

                IPAddress iPAddress = Client.staticServerIP;

                udpClient = new UdpClient(Consts.clientPort);
                serverEndPoint = new IPEndPoint(iPAddress, Consts.serverPort);
                udpClient.BeginReceive(Receive, null);

                Client.debugMsg += " Succes";
            }
            catch (Exception e)
            {
                exceptionError = " Error " + e.Message;
            }
            Client.debugMsg += " After Connect";
        }

        public void Connect(IPAddress adress)
        {
            try
            {
                exceptionError = "UDPProtocolClient Error"; //set default text
                Client.debugMsg += " On Connect";
                //adress = IPAddress.Any;

                udpClient = new UdpClient(Consts.clientPort);
                serverEndPoint = new IPEndPoint(adress, Consts.serverPort);
                udpClient.BeginReceive(Receive, null);

                Client.debugMsg += " Succes";
            }
            catch (Exception e)
            {
                exceptionError = " Error " + e.Message;
            }
            Client.debugMsg += " After Connect";
        }
        //public void SetServerIPEndPoint(IPEndPoint ipEndPoint)
        //{
        //    serverEndPoint = ipEndPoint;
        //}
        /// <summary>
        /// Sets the ip of the server to connect to
        /// </summary>
        /// <param name="ip"></param>
        public void SetServerIP(IPAddress ip)
        {
            if (serverEndPoint == null)
            {
                serverEndPoint = new IPEndPoint(ip, Consts.serverPort);
            }
            else
            {
                serverEndPoint.Address = ip;
            }

        }
        /// <summary>
        /// Closes a socket
        /// </summary>
        public void Disconnect()
        {
            udpClient.Close();
        }

        void Receive(IAsyncResult asyncResult)
        {
            try
            {
                IPEndPoint iPEndPoint = null;
                byte[] data = udpClient.EndReceive(asyncResult, ref iPEndPoint);
                string message = System.Text.Encoding.UTF8.GetString(data);
                Read(message, iPEndPoint);
            }
            catch (Exception e)
            {
                exceptionError = "Recive Error " + e.Message;
            }
            udpClient.BeginReceive(Receive, null);
        }
        /// <summary>
        /// When a new UDP message arrives
        /// </summary>
        /// <param name="asyncResult"></param>
        public void Read(string message, IPEndPoint ipEndPoint)
        {
            Client.instance.ReadUDP(message, ipEndPoint);
        }
        /// <summary>
        /// Send a message to the server
        /// </summary>
        /// <param name="message">The message</param>
        public void Send(string message)
        {
            try
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
                udpClient.Send(data, data.Length, serverEndPoint);
                sendCount++;
            }
            catch (Exception e)
            {
                exceptionError = e.Message;
            }
        }
        //public void Broadcast(string message)
        //{
        //    byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
        //    broadcastClient.Send(data, data.Length);
        //}
    }
}