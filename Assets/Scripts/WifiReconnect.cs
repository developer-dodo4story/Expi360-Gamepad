using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedDodoServer;
using UnityEngine.UI;
using System;
using GamePad;

public class WifiReconnect : MonoBehaviour
{
    public static WifiReconnect Instance;
    public string ipToServer = "";
    public bool pingFinished;

    public Text pingAddres;
    public Text pingLog;

    public Text UDPConnectedClientErrors;
    public Text clientDebug;
    public Text counterClient;
    public Text counterSend;
    public Text UDPProtocolClientErrors;
    public Text id;


    private Coroutine breakingCouroutine;
    private bool breakPing = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        pingFinished = true;
    }

    public void SetID(string value)
    {
        if (int.TryParse(value, out int staticID))
        {
            Debug.Log("Trying to set ID: " + staticID);
            Player.instance.playerInput.clientData.clientID = staticID;
        }
        else
        {
            Debug.Log("Parse return false");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //GetIpServer();
        //CheckPing(ipToServer);
    }

    private bool GetIpServer()
    {

        if (Client.instance != null)
        {
            UDPConnectedClientErrors.text = UDPConnectedClient.exceptionError;
            clientDebug.text = Client.debugMsg;
            counterClient.text = Client.messageCount.ToString();
            UDPProtocolClientErrors.text = UDPProtocolClient.exceptionError;
            id.text = Player.instance.playerInput.clientData.clientID.ToString();
            counterSend.text = UDPProtocolClient.sendCount.ToString();

            if (Client.instance.ServerIP != null)
            {
                ipToServer = Client.instance.ServerIP.ToString();
                pingAddres.text = ipToServer;
                Debug.Log("Ip are set!");
                return true;
            }
            else
            {
                Debug.Log("Client.instance.ServerIP == null");
                pingAddres.text = "Null";
                return false;
            }
        }
        else
        {
            Debug.Log("Client.instance == null");
            pingAddres.text = "Client.instance == null";
            clientDebug.text = "Client.instance == null";
            return false;
        }
    }

    public void Disconnect()
    {
        Client.instance.udpInitialized = false;
        UDPProtocolClient.instance.Disconnect();
    }

    public void Connect()
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

    public void CheckPing(string ip)
    {
        if (pingFinished && !string.IsNullOrEmpty(ip))
        {
            pingFinished = false;
            StartCoroutine(StartPing(ip));
        }
    }

    IEnumerator StartPing(string ip)
    {
        breakPing = false;
        breakingCouroutine = StartCoroutine(BreakPing(5f));
        WaitForSeconds f = new WaitForSeconds(0.05f);
        UnityEngine.Ping p = new UnityEngine.Ping(ip);
        while (p.isDone == false && breakPing == false)
        {
            yield return f;
        }
        PingFinished(p);
    }

    IEnumerator BreakPing(float time)
    {
        yield return new WaitForSeconds(time);
        breakPing = true;
    }

    public void PingFinished(UnityEngine.Ping p)
    {
        // stuff when the Ping p has finshed....
        pingFinished = true;
        StopCoroutine(breakingCouroutine);
        if (!breakPing)
        {
            Debug.Log("Ping finished at time: " + p.time);
            pingLog.text = "Ping finished at time: " + p.time;
        }
        else
        {
            pingLog.text = "Ping breaked";
            Debug.Log("Ping breaked");
        }
    }
}
