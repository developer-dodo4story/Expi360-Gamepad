using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedDodoServer;
using System.Net;
using System;
using UnityEngine.UI;

namespace GamePad
{

    /// <summary>
    /// Class managing player input and sending it to server
    /// </summary>
    public class Player : Singleton<Player>
    {
        //public Text debug;

        [HideInInspector]
        public bool playerSetName = false;
        /// <summary>
        /// Input of the player
        /// </summary>        
        public PlayerInput playerInput { get; set; }
        /// <summary>
        /// Name of the client
        /// </summary>
        string clientName;
        /// <summary>
        /// Client ip
        /// </summary>
        string ipAddress;
        /// <summary>
        /// Current ButtonsLayout
        /// </summary>

        private float totalTime = 0f;
        private readonly float timeDelay = 0.1f; //60fps odpowiada 0.016f

        public ButtonsLayoutOptions ButtonsLayoutOption { get; set; }
        /// <summary>
        /// Are the game buttons ready to be clicked?
        /// </summary>
        [HideInInspector]
        public bool buttonsEnabled;
        void Start()
        {
            string clientName = AdminPanel.GetPlayerName();
            int id = AdminPanel.GetID();
            playerInput = new PlayerInput(clientName);
            playerInput.clientData.clientID = id;
            InitUDP();
        }
        /// <summary>
        /// Sets client name
        /// </summary>
        /// <param name="clientName">Client name</param>
        public void SetClientName(string clientName)
        {
            playerSetName = true;
            playerInput.clientData.name = clientName;
        }
        /// <summary>
        /// Looks for a server over UDP
        /// </summary>
        public void InitUDP()
        {
            //UIControllerClient.instance.Log("Looking for a server...");
            //UIControllerClient.instance.Log("Show buttons layouts");
            UIControllerClient.instance.Log(UIControllerClient.LOGTEXT.Show);
        }

        void Update()
        {
            EnableGamepadLayout();

            if (!buttonsEnabled) return;

            playerInput.start = JoystickInput.GetKey(JoystickKeyCode.START);
            playerInput.back = JoystickInput.GetKey(JoystickKeyCode.BACK);
            playerInput.guide = JoystickInput.GetKey(JoystickKeyCode.GUIDE);
            playerInput.leftBumper = JoystickInput.GetKey(JoystickKeyCode.L_BUMPER);
            playerInput.rightBumper = JoystickInput.GetKey(JoystickKeyCode.R_BUMPER);
            playerInput.leftTrigger = JoystickInput.LeftTrigger;
            playerInput.rightTrigger = JoystickInput.RightTrigger;
            playerInput.aButton = JoystickInput.GetKey(JoystickKeyCode.A);
            playerInput.bButton = JoystickInput.GetKey(JoystickKeyCode.B);
            playerInput.xButton = JoystickInput.GetKey(JoystickKeyCode.X);
            playerInput.yButton = JoystickInput.GetKey(JoystickKeyCode.Y);

            if (ButtonsLayoutOption == ButtonsLayoutOptions.BellyRumble_ButtonsOnly)
            {
                if (JoystickInput.GetKey(JoystickKeyCode.LEFT))
                    playerInput.leftStick.Horizontal = -1f;
                else if (JoystickInput.GetKey(JoystickKeyCode.RIGHT)) playerInput.leftStick.Horizontal = 1f;
                else playerInput.leftStick.Horizontal = 0f;
                if (JoystickInput.GetKey(JoystickKeyCode.UP)) playerInput.leftStick.Vertical = 1f;
                else if (JoystickInput.GetKey(JoystickKeyCode.DOWN)) playerInput.leftStick.Vertical = -1f;
                else playerInput.leftStick.Vertical = 0f;
            }
            else
            {
                playerInput.leftStick.Horizontal = JoystickInput.LeftStick.Horizontal;
                playerInput.leftStick.Vertical = JoystickInput.LeftStick.Vertical;
                playerInput.rightStick.Horizontal = JoystickInput.RightStick.Horizontal;
                playerInput.rightStick.Vertical = JoystickInput.RightStick.Vertical;
                playerInput.directionalPad.Horizontal = JoystickInput.DirectionalPad.Horizontal;
                playerInput.directionalPad.Vertical = JoystickInput.DirectionalPad.Vertical;
            }



            if (IsJoystickConnected())
            {
                try
                {
                    //W edytorze przycisk start kontrolera xbox one jest mapowany jako button 7, 
                    //a gdy już wgramy aplikację na telefon ten sam przycisk jest mapowany przez button 10.
#if UNITY_ANDROID
                    playerInput.start = Input.GetKey("joystick button 10");
# endif

#if UNITY_EDITOR
                    playerInput.start = Input.GetButton("Start");
#endif

#if !UNITY_ANDROID
                    playerInput.start = Input.GetButton("Start");
#endif

                    playerInput.leftStick.Horizontal = Input.GetAxis("HorizontalGamepad");
                    playerInput.leftStick.Vertical = -Input.GetAxis("VerticalGamepad");

                    playerInput.aButton = Input.GetButton("XboxA");
                    playerInput.bButton = Input.GetButton("XboxB");
                    playerInput.xButton = Input.GetButton("XboxX");
                    playerInput.yButton = Input.GetButton("XboxY");
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }

                //debug.text = string.Format("Start: {0}, H {1}, V {2}, A {3}, B {4}, X {5}, Y {6}",
                //    playerInput.start, playerInput.leftStick.Horizontal, playerInput.leftStick.Vertical,
                //    playerInput.aButton, playerInput.bButton, playerInput.xButton, playerInput.yButton);

                /*
                Debug.LogFormat("Start: {0}, H {1}, V {2}, A {3}, B {4}, X {5}, Y {6}",
                    playerInput.start, playerInput.leftStick.Horizontal, playerInput.leftStick.Vertical,
                    playerInput.aButton, playerInput.bButton, playerInput.xButton, playerInput.yButton);
                    */
                if (!GamePadClient.Instance.udpInitialized)
                {
                    if (Client.instance.ServerIP != null)
                    {
                        UDPProtocolClient.instance.Connect(Client.instance.ServerIP);
                    }
                    else
                    {
                        UDPProtocolClient.instance.Connect();
                    }
                    Client.instance.udpInitialized = true;
                }
            }

            totalTime += Time.deltaTime;

            if (Input.touchCount > 0)
            {
                finger1 = Input.GetTouch(0);
                if (finger1.phase == TouchPhase.Began)
                {
                    totalTime += timeDelay * 2f; // *2 aby na pewno przekroczyc zakres.
                    Debug.Log("Reset time 1");
                }
            }
            if (Input.touchCount > 1)
            {
                finger2 = Input.GetTouch(1);
                if (finger2.phase == TouchPhase.Began)
                {
                    totalTime += timeDelay * 2f; // *2 aby na pewno przekroczyc zakres.
                }
            }

            if (totalTime > timeDelay && GamePadClient.Instance.udpInitialized)
            {
                if (playerInput.counter1 >= int.MaxValue)
                {
                    playerInput.counter1 = 0;
                    playerInput.counter2++;
                }
                string json = JsonUtility.ToJson(playerInput);
                GamePadClient.Instance.SendToServerUDP(json);
                //Debug.Log(totalTime);
                playerInput.counter1++;
                totalTime = 0f;
            }
        }

        private Touch finger1;
        private Touch finger2;

        private string[] connectedJoystick;
        private bool IsJoystickConnected()
        {
            connectedJoystick = Input.GetJoystickNames();
            for (int i = 0; i < connectedJoystick.Length; i++)
            {
                if (connectedJoystick[i].Length > 0)
                {
                    isJoystickConnected = true;
                    return true;
                }
            }
            isJoystickConnected = false;
            return false;
        }

        private bool isJoystickConnected;
        private bool activeGamepad;
        private void EnableGamepadLayout()
        {
            if (isJoystickConnected)
            {
                if (!activeGamepad)
                {
                    GamePadClient.Instance.SetLayout_Gamepad();
                    activeGamepad = true;
                    ToogleActivateObject.OnGamePadEnabled?.Invoke();
                }
            }
            else
            {
                if (activeGamepad)
                {
                    activeGamepad = false;
                    GamePadClient.Instance.SetLayout_Gamepad();
                    ToogleActivateObject.OnGamePadDisabled?.Invoke();
                }
            }
        }

        /// <summary>
        /// Save a skin as a preferenced one for this game, in PlayerPrefs
        /// </summary>
        /// <param name="key">Game as a string</param>
        /// <param name="skin">Skin to be saved</param>
        public void SavePreferencedSkin(string key, Skin skin)
        {
            string json = JsonUtility.ToJson(skin.skinData);
            if (PlayerPrefs.HasKey(key)) PlayerPrefs.DeleteKey(key);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }
        /// <summary>
        /// Get a preferenced skin for this game
        /// </summary>
        /// <param name="key">Game as a string</param>
        /// <returns></returns>
        public Skin GetPreferencedSkin(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key, "");

                if (json == "")
                    return null;
                else
                {
                    SkinData skinData = JsonUtility.FromJson<SkinData>(json);
                    Skin skin = new Skin();
                    skin.skinData = skinData;
                    return skin;
                }
            }
            else return null;
        }
    }
}