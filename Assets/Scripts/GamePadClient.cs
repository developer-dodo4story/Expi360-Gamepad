using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using EnhancedDodoServer;
using System;

namespace GamePad
{
    /// <summary>
    /// High level client class
    /// </summary>
    public class GamePadClient : Client, IClientUDP
    {
        /// <summary>
        /// Singleton
        /// </summary>
        public static GamePadClient Instance;
        protected override void Awake()
        {
            base.Awake();
            if (Instance == null)
                Instance = (GamePadClient)FindObjectOfType(typeof(GamePadClient));
            else
                Destroy(gameObject);
            debugMsg += "Awake ";
        }
        private void Start()
        {
            debugMsg += "Jestem w Start. ";

            //IPAddress.TryParse(defaultIpToServer, out IPAddress staticAdress);
            //if (staticAdress != null)
            //{
            //    ServerIP = staticAdress;
            //    UDPProtocolClient.instance.SetServerIP(ServerIP);
            //    udpInitialized = true;
            //}

            ConnectUDP();
        }

        private void SetEmptyLayoutState()
        {
            currentGame = Consts.Game.Empty;
            //UIControllerClient.instance.Log("Hide buttons layouts");
            UIControllerClient.instance.Log(UIControllerClient.LOGTEXT.Hide);
        }

        public void SetLayout_BellyRumble()
        {
            if (currentGame == Consts.Game.BellyRumble)
            {
                SetEmptyLayoutState();
            }
            else
            {
                currentGame = Consts.Game.BellyRumble;
            }
            SelectedElement.OnGameSelected?.Invoke(currentGame);
        }

        public void SetLayout_PotionMaster()
        {
            if (currentGame == Consts.Game.PotionsMaster)
            {
                SetEmptyLayoutState();
            }
            else
            {
                currentGame = Consts.Game.PotionsMaster;
            }
            SelectedElement.OnGameSelected?.Invoke(currentGame);
        }

        public void SetLayout_CityUnderAttack()
        {
            if (currentGame == Consts.Game.CityUnderAttack)
            {
                SetEmptyLayoutState();
            }
            else
            {
                currentGame = Consts.Game.CityUnderAttack;
            }
            SelectedElement.OnGameSelected?.Invoke(currentGame);
        }

        public void SetLayout_FieldsOfGlory()
        {
            if (currentGame == Consts.Game.FieldsOfGlory)
            {
                SetEmptyLayoutState();
            }
            else
            {
                currentGame = Consts.Game.FieldsOfGlory;
            }
            SelectedElement.OnGameSelected?.Invoke(currentGame);
        }

        public void SetLayout_OgrodEmocji()
        {
            if (currentGame == Consts.Game.Empatki)
            {
                SetEmptyLayoutState();
            }
            else
            {
                currentGame = Consts.Game.Empatki;
            }
            SelectedElement.OnGameSelected?.Invoke(currentGame);
        }

        public void SetLayout_Submarine()
        {
            if (currentGame == Consts.Game.Submarine)
            {
                SetEmptyLayoutState();
            }
            else
            {
                currentGame = Consts.Game.Submarine;
            }
            SelectedElement.OnGameSelected?.Invoke(currentGame);
        }

        public void SetLayout_Gamepad()
        {
            if (currentGame == Consts.Game.Gamepad)
            {
                SetEmptyLayoutState();
            }
            else
            {
                currentGame = Consts.Game.Gamepad;
            }
            SelectedElement.OnGameSelected?.Invoke(currentGame);
        }

        /// <summary>
        /// Opens a udp socket
        /// </summary>
        public void ConnectUDP()
        {
            base.InitUDP();
        }
        /// <summary>
        /// Sends a message over UDP to the server
        /// </summary>
        /// <param name="message">The message</param>
        public void SendToServerUDP(string message)
        {
            base.SendUDP(message);
        }
        /// <summary>
        /// On game disconnect resets clientID and server name and its ip
        /// </summary>
        /// <param name="name"></param>
        public override void RefreshServerNameAndIP(string name)
        {
            base.RefreshServerNameAndIP(name);
            //Player.instance.playerInput.clientData.clientID = -1;
        }

        public override void Update()
        {
            base.Update();

            if (Player.instance.playerInput.clientData.clientID == -1)
            {
                string[] sequences = Consts.SequenceSplit(udpMessage);
                foreach (string sequence in sequences)
                {
                    string[] words = Consts.WordSplit(sequence);
                    if (words[0] == Consts.Words.id)
                    {
                        // mozliwy blad z parse
                        bool success = int.TryParse(words[1], out int value);

                        if (success)
                        {
                            Player.instance.playerInput.clientData.clientID = value;
                            if (!Player.instance.playerSetName)
                            {
                                Player.instance.playerInput.clientData.name = "Player " + (value + 1).ToString();
                            }
                            UIControllerClient.instance.SetPlayerName(Player.instance.playerInput.clientData.name);
                        }

                    }
                }
            }
        }


        /// <summary>
        /// Invokes when a new udp message arrives
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="ipEndPoint">IPEndPoint where the message came from</param>
        public override void ReadUDP(string message, IPEndPoint ipEndPoint)
        {
            base.ReadUDP(message, ipEndPoint);
            //if (Player.instance.playerInput.clientData.clientID == -1)
            //{
            //    string[] sequences = Consts.SequenceSplit(message);
            //    foreach (string sequence in sequences)
            //    {
            //        string[] words = Consts.WordSplit(sequence);
            //        if (words[0] == Consts.Words.id)
            //        {
            //            // mozliwy blad z parse
            //            bool success = int.TryParse(words[1], out int value);

            //            if (success)
            //            {
            //                Player.instance.playerInput.clientData.clientID = value;
            //                if (!Player.instance.playerSetName)
            //                {
            //                    Player.instance.playerInput.clientData.name = "Player " + (value + 1).ToString();
            //                }
            //                UIControllerClient.instance.SetPlayerName(Player.instance.playerInput.clientData.name);
            //            }

            //        }
            //    }
            //}
        }
    }
}