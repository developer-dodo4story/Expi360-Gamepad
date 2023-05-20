using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using EnhancedDodoServer;
using GamePad;
using System;

public class AdminPanel : MonoBehaviour
{
    public static string clientIPAddressPlayerPrefs = "ClientIPAdmin";
    public static string clientNamePlayerPrefs = "ClientNameAdmin";
    public static string clientIDPlayerPrefs = "ClientIDAdmin";

    public GameObject PasswordContent;
    public GameObject SettingsContent;
    public InputField passwordField;

    public InputField iPAddress;
    public InputField playerName;
    public InputField playerId;

    public Text currentIP;
    public Text currentName;
    public Text currentID;

    public Text recive;
    public Text send;

    private IPAddress ipToServer;
    private int id = -1;

    private readonly string Password = "DoDo";

    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        LoadValues();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        if (send != null && send.IsActive())
        {
            send.text = UDPProtocolClient.sendCount.ToString();
        }
        if (recive != null && recive.IsActive())
        {
            recive.text = Client.messageCount.ToString();
        }
    }

    public void UpdateValues()
    {
        if (Client.staticServerIP != null)
        {
            currentIP.text = Client.staticServerIP.ToString();
        }
        currentID.text = Player.instance.playerInput.clientData.clientID.ToString();
        currentName.text = Player.instance.playerInput.clientData.name;
    }

    public static int GetID()
    {
        int id = PlayerPrefs.GetInt(clientIDPlayerPrefs, -1);
        return id;
    }

    public static string GetPlayerNameWithID()
    {
        string name = PlayerPrefs.GetString(clientNamePlayerPrefs, "Player");
        int id = PlayerPrefs.GetInt(clientIDPlayerPrefs, -1);
        if (id != -1)
        {
            name += " " + id;
        }
        return name;
    }

    public static string GetPlayerName()
    {
        string name = PlayerPrefs.GetString(clientNamePlayerPrefs, "Player");
        return name;
    }

    private void LoadIPAddress()
    {
        string ip = PlayerPrefs.GetString(clientIPAddressPlayerPrefs);
        if (IPAddress.TryParse(ip, out ipToServer))
        {
            Client.staticServerIP = ipToServer;
            currentIP.text = ipToServer.ToString();
            iPAddress.text = ipToServer.ToString();
        }
        else
        {
            currentIP.text = "Null";
            iPAddress.text = "Null";
        }
    }

    private IEnumerator SetPlayerName(String name)
    {
        //Czekam az instancja bedzie gotowa
        Debug.Log("Czekam na instacje");
        while (Player.instance == null || Player.instance.playerInput == null)
        {
            yield return null;
        }
        Debug.Log("Player Name ustawione: " + name);
        if (id != -1)
        {
            Player.instance.SetClientName(name + " " + id);
        }
        else
        {
            Player.instance.SetClientName(name);
        }
    }

    private void LoadName()
    {
        string name = PlayerPrefs.GetString(clientNamePlayerPrefs, "Player");
        if (id != -1)
        {
            currentName.text = name + " " + id;
        }
        else
        {
            currentName.text = name;
        }
        playerName.text = name;
        StartCoroutine(SetPlayerName(name));
    }

    private void LoadID()
    {
        int id = PlayerPrefs.GetInt(clientIDPlayerPrefs, -1);
        if (id == -1)
        {
            this.id = id;
            currentID.text = "Null";
            playerId.text = "Null";
        }
        else
        {
            this.id = id;
            currentID.text = id.ToString();
            playerId.text = id.ToString();
        }
    }

    private void SaveName()
    {
        if (playerName != null)
        {
            PlayerPrefs.SetString(clientNamePlayerPrefs, playerName.text);
            if (id != -1)
            {
                Player.instance.SetClientName(playerName.text + " " + id);
                UIControllerClient.instance.nameInputField.text = playerName.text + " " + id;
            }
            else
            {
                Player.instance.SetClientName(playerName.text);
                UIControllerClient.instance.nameInputField.text = playerName.text;
            }
        }
    }

    private void SaveID()
    {
        if (playerId != null)
        {
            if (int.TryParse(playerId.text, out int id))
            {
                PlayerPrefs.SetInt(clientIDPlayerPrefs, id);
                this.id = id;
                currentID.text = id.ToString();
                Player.instance.playerInput.clientData.clientID = id;
                UIControllerClient.instance.nameInputField.text = playerName.text + " " + id;
            }

        }
    }

    private void SaveIP()
    {
        if (iPAddress != null)
        {
            if (IPAddress.TryParse(iPAddress.text, out ipToServer))
            {
                currentIP.text = ipToServer.ToString();
                PlayerPrefs.SetString(clientIPAddressPlayerPrefs, ipToServer.ToString());
                Client.staticServerIP = ipToServer;
            }
            else
            {
                ipToServer = null;
                currentIP.text = "Null";
            }
        }
    }

    public void ShowSettingsPanel()
    {
        PasswordContent.SetActive(false);
        SettingsContent.SetActive(true);
        UpdateValues();
    }

    public void Logout()
    {
        passwordField.text = "";
        PasswordContent.SetActive(true);
        SettingsContent.SetActive(false);
    }

    public void LogIN()
    {
        if (string.Equals(passwordField.text, Password))
        {
            Debug.LogFormat("{0} == {1}", passwordField.text, Password);
            ShowSettingsPanel();
        }
        else
        {
            Debug.Log("Not equals");
        }
    }

    public void SaveValues()
    {
        SaveIP();
        SaveID();
        SaveName();
        UpdateValues();
    }

    private void LoadValues()
    {
        LoadIPAddress();
        LoadID();
        LoadName();
    }
}
