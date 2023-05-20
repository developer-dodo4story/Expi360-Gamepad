using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using EnhancedDodoServer;
using UnityEngine.Events;
using System;
using GamePad;

namespace EnhancedDodoServer
{
    /// <summary>
    /// Enum describing protocol used for Network Discovery
    /// </summary>
    public enum NetworkDiscoveryMethod { None, OverTCP, OverUDP }
    /// <summary>
    /// Class representing a client on the client side
    /// </summary>
    public class Client : Singleton<Client>
    {
        public static int messageCount = 0;
        public static IPAddress staticServerIP;
        public static string debugMsg = "ClientDebug Message";

        public Consts.Game currentGame;
        /// <summary>
        /// Counts how long there has been no message from server
        /// </summary>
        float serverDisconnectTimer = 0f;
        /// <summary>
        /// Max time without a message from server after which client disconnects
        /// </summary>
        float maxServerDisconnectTime = 2f;
        /// <summary>
        /// Is it possible to send and receive udp messages
        /// </summary>
        [HideInInspector]
        public bool udpInitialized = false;
        /// <summary>
        /// Is it possible to send and receive tcp messages
        /// </summary>
        [HideInInspector]
        public bool tcpInitialized = false;
        /// <summary>
        /// How to discover network
        /// </summary>
        protected NetworkDiscoveryMethod networkDiscoveryMethod;
        /// <summary>
        /// Name of the game server is playing
        /// </summary>
        string gameName;
        /// <summary>
        /// The game server played previously, used to determine when the game has changed
        /// </summary>
        string prevGameName;
        /// <summary>
        /// IP of the server to connect to
        /// </summary>
        private IPAddress serverIP;
        /// <summary>
        /// Property of the server ip address. Sets serverIP on UDPProtocolClient
        /// </summary>
        public IPAddress ServerIP
        {
            get
            {
                //string savedIp = PlayerPrefs.GetString(ipToServer, defaultIpToServer);
                //if (!string.Equals(savedIp, defaultIpToServer))
                //{
                //    IPAddress.TryParse(savedIp, out serverIP);
                //}
                return staticServerIP;
                return serverIP;
            }
            set
            {
                serverIP = value;
                //if (serverIP != null)
                //{
                //    PlayerPrefs.SetString(ipToServer, serverIP.ToString());
                //}

                if (!udpInitialized && serverIP != null)
                {
                    udpInitialized = true;
                    UDPProtocolClient.instance.SetServerIP(serverIP); // inita robie w gamepadClient
                    if (onUDPInitialized != null)
                    {
                        onUDPInitialized();
                    }
                }
                else if (serverIP == null)
                {
                    udpInitialized = false;
                }
            }
        }
        /// <summary>
        /// Name of the server
        /// </summary>
        public string ServerName { get; set; }
        /// <summary>
        /// Public event invoked when client has found a server through Network Discovery
        /// </summary>
        public UnityAction<IPAddress> onNetworkDiscoveryComplete;
        /// <summary>
        /// Public event invoked when udp socket has been opened
        /// </summary>
        public UnityAction onUDPInitialized;
        /// <summary>
        /// Public event invoked when tcp socket has been opened
        /// </summary>
        public UnityAction onTCPInitialized;
        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// When the server name changes
        /// </summary>
        /// <param name="name">Server name</param>
        public virtual void RefreshServerNameAndIP(string name)
        {
            ServerName = name;
            //ServerIP = null;
        }
        /// <summary>
        /// Opens tcp socket
        /// </summary>
        protected virtual void InitTCP()
        {
            if (TCPProtocolClient.instance == null)
            {
                Debug.LogError("Missing component: TCPProtocolClient");
                return;
            }
            if (ServerIP == null) Debug.LogError("No serverIP, network discovery possibly not completed");
            TCPProtocolClient.instance.Connect(ServerIP);
            tcpInitialized = true;

            if (onTCPInitialized != null)
                onTCPInitialized();
        }
        /// <summary>
        /// Opens udp socket
        /// </summary>
        protected virtual void InitUDP()
        {
            if (UDPProtocolClient.instance == null)
            {
                Debug.LogError("Missing component: UDPProtocolClient");
                return;
            }
            //if (serverIP == null) Debug.LogError("No serverIP, network discovery possibly not completed");
            //UDPProtocolClient.instance.Connect(serverIP);
            if (staticServerIP != null)
            {
                UDPProtocolClient.instance.Connect();
            }
            else
            {
                Debug.Log("Static IP null");
            }
        }
        
        public string udpMessage;
        public IPEndPoint udpIpEndPoint;
        /// <summary>
        /// Invoked when a new UDP message arrives
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="_ipEndPoint">IPEndPoint where the message came from</param>
        public virtual void ReadUDP(string message, IPEndPoint _ipEndPoint)
        {
            udpMessage = message;
            udpIpEndPoint = _ipEndPoint;

            messageCount++;
            if (messageCount >= int.MaxValue)
            {
                messageCount = 0;
            }

            ////debugMsg = message;
            //Debug.Log(message);
            //string[] sequences = Consts.SequenceSplit(message);
            //bool correctServer = false;
            //foreach (string sequence in sequences)
            //{
            //    string[] words = Consts.WordSplit(sequence);
            //    if (words[0] == Consts.Words.serverName)
            //    {
            //        serverDisconnectTimer = 0f;
            //        if (words[1].Equals(ServerName))
            //        {
            //            HandlePing();
            //            correctServer = true;
            //            //ServerIP = _ipEndPoint.Address;
            //            string tmp1 = "Game: " + gameName + "\n";
            //            string tmp2 = "Ping: " + ping + " m: " + pingMax + " fps: " + fps;
            //            string tmp3 = " ButtonLayout: " + UIControllerClient.instance.currentButtonsLayout?.option.ToString();
            //            GamePad.UIControllerClient.instance.Log(tmp1 + tmp2 + tmp3);
            //        }
            //        else return;
            //    }
            //    else if (correctServer && words[0] == Consts.Words.game)
            //    {
            //        //if (gameName != words[1])
            //        //{
            //        //    OnGameChange(words[1]);
            //        //    print("2");
            //        //}
            //        gameName = words[1];
            //    }
            //}
        }

        private float pingMax = -1;
        private float ping = -1;
        private readonly System.Diagnostics.Stopwatch swPing = new System.Diagnostics.Stopwatch();
        private void HandlePing()
        {
            swPing.Stop();
            ping = swPing.ElapsedMilliseconds;

            if (pingMax < ping)
            {
                pingMax = ping;
            }

            swPing.Reset();
            swPing.Start();
        }

        public void SetMaxPingToZero()
        {
            pingMax = 0f;
        }

        private string fps = "-1";
        private float detlaTime = 0f;
        private readonly System.Diagnostics.Stopwatch swFPS = new System.Diagnostics.Stopwatch();
        private void HandelFPS()
        {
            swFPS.Stop();
            detlaTime += (Time.unscaledDeltaTime - detlaTime) * 0.1f;
            fps = (1f / detlaTime).ToString("f1");
            swFPS.Reset();
            swFPS.Start();
        }

        protected virtual void OnGameChange(string newGame)
        {
            LoadSkin(newGame);
        }
        protected void LoadSkin(string game)
        {
            UIControllerClient.instance.SwitchSkin(game);
        }

        public virtual void Update()
        {
            debugMsg = udpMessage;
            //Debug.Log(udpMessage);

            gameName = currentGame.ToString();
            //string[] sequences = Consts.SequenceSplit(udpMessage);
            //bool correctServer = false;
            //foreach (string sequence in sequences)
            //{
            //    string[] words = Consts.WordSplit(sequence);
            //    if (words[0] == Consts.Words.serverName)
            //    {
            //        serverDisconnectTimer = 0f;
            //        if (words[1].Equals(ServerName))
            //        {
            //            //HandlePing();
            //            correctServer = true;

            //            //if (ServerIP == null && udpIpEndPoint != null)
            //            //{
            //            //    ServerIP = udpIpEndPoint.Address;
            //            //}

            //            string tmp1 = "Game: " + gameName + "\n";
            //            //string tmp2 = "Ping: " + ping + " m: " + pingMax + " fps: " + fps;
            //            string tmp3 = " ButtonLayout: ";
            //            if (UIControllerClient.instance.currentButtonsLayout != null)
            //            {
            //                tmp3 += UIControllerClient.instance.currentButtonsLayout.option.ToString();
            //            }
            //            //GamePad.UIControllerClient.instance.Log(tmp1 + tmp2 + tmp3);
            //            GamePad.UIControllerClient.instance.Log(tmp1 + tmp3);
            //        }
            //        else return;
            //    }
            //    else if (correctServer && words[0] == Consts.Words.game)
            //    {
            //        //if (gameName != words[1])
            //        //{
            //        //    OnGameChange(words[1]);
            //        //    print("2");
            //        //}
            //        gameName = words[1];
            //    }
            //}

            //serverDisconnectTimer += Time.deltaTime;
            //if (serverDisconnectTimer >= maxServerDisconnectTime)
            //{
            //    RefreshServerNameAndIP(ServerName);
            //    GamePad.UIControllerClient.instance.Log("Looking for a server...");
            //}



            if (gameName != prevGameName)
            {
                OnGameChange(gameName);
            }
            prevGameName = gameName;

            //OnGameChange(Consts.Game.CityUnderAttack.ToString());

            //HandelFPS();
        }
        /// <summary>
        /// Invoked when a new TCP message arrives
        /// </summary>
        /// <param name="message"></param>
        public virtual void ReadTCP(string message)
        {

        }
        /// <summary>
        /// Sends a message over UDP
        /// </summary>
        /// <param name="message">The message</param>
        protected virtual void SendUDP(string message)
        {
            if (udpInitialized)
            {
                UDPProtocolClient.instance.Send(message);
            }
        }
        /// <summary>
        /// Sends a message over TCP
        /// </summary>
        /// <param name="message">The message</param>
        protected virtual void SendTCP(string message)
        {
            if (tcpInitialized)
            {
                TCPProtocolClient.instance.Send(message);
            }
        }
    }
}