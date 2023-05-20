using EnhancedDodoServer;
using GamePad;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToogleActivateObject : MonoBehaviour
{
    public static Action OnGamePadEnabled;
    public static Action OnGamePadDisabled;

    [Tooltip("List of objects that are activated after calling the OnToogle function.")]
    public GameObject[] gameObjectsToActivate;
    [Tooltip("List of objects that are deactivated after calling the OnToogle function.")]
    public GameObject[] gameObjectsToDeactivate;
    public bool startState;

    private bool currentState;

    private void Awake()
    {
        currentState = startState;
        foreach (GameObject obj in gameObjectsToActivate)
        {
            obj.SetActive(startState);
        }
        foreach (GameObject obj in gameObjectsToDeactivate)
        {
            obj.SetActive(!startState);
        }
    }

    private void OnEnable()
    {
        OnGamePadEnabled += OnGamepadEnable;
        OnGamePadDisabled += OnGamepadDisabled;
    }

    private void OnDisable()
    {
        OnGamePadEnabled -= OnGamepadEnable;
        OnGamePadDisabled -= OnGamepadDisabled;
    }

    public void OnGamepadEnable()
    {
        foreach (GameObject obj in gameObjectsToActivate)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in gameObjectsToDeactivate)
        {
            obj.SetActive(false);
        }
    }

    public void OnGamepadDisabled()
    {
        foreach (GameObject obj in gameObjectsToActivate)
        {
            obj.SetActive(currentState);
            UIControllerClient.instance.Log(UIControllerClient.LOGTEXT.Hide);
        }
        foreach (GameObject obj in gameObjectsToDeactivate)
        {
            obj.SetActive(!currentState);
        }
    }

    public void OnToogle()
    {
        currentState = !currentState;
        foreach (GameObject obj in gameObjectsToActivate)
        {
            obj.SetActive(currentState);
            UIControllerClient.instance.Log(UIControllerClient.LOGTEXT.Hide);
        }
        foreach (GameObject obj in gameObjectsToDeactivate)
        {
            obj.SetActive(!currentState);
        }

        if (currentState == false && Client.instance.currentGame != Consts.Game.Empty)
        {
            UIControllerClient.instance.Log(UIControllerClient.LOGTEXT.ShowLayout, Client.instance.currentGame.ToString());
        }
        else if (currentState == true && Client.instance.currentGame != Consts.Game.Empty)
        {
            UIControllerClient.instance.Log(UIControllerClient.LOGTEXT.HideLayout, Client.instance.currentGame.ToString());
        }
        else if (currentState == false && Client.instance.currentGame == Consts.Game.Empty)
        {
            UIControllerClient.instance.Log(UIControllerClient.LOGTEXT.Show);
        }

        if (Client.instance.currentGame != Consts.Game.Empty)
        {
            foreach (GameObject obj in gameObjectsToDeactivate)
            {
                CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0f;
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }
            }
        }
        else
        {
            foreach (GameObject obj in gameObjectsToDeactivate)
            {
                CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
            }
        }
    }
}
