using EnhancedDodoServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using GamePad;

public class SelectedElement : MonoBehaviour
{
    public static Action<Consts.Game> OnGameSelected;

    public string displayGameLayout;
    public Consts.Game gameLayout;

    private readonly float timetoAutoToogle = 0.5f;
    private ToogleActivateObject toogleActivateObject;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        toogleActivateObject = GetComponentInParent<ToogleActivateObject>();
        canvasGroup.alpha = 0f;
    }

    public void OnSelectedGame(Consts.Game gameLayout)
    {
        if (this.gameLayout == gameLayout)
        {
            canvasGroup.alpha = 1f;
            StartCoroutine(StartToogle(timetoAutoToogle));
            //UIControllerClient.instance.Log("Hide buttons layouts" + "\nLayout: " + displayGameLayout);
            UIControllerClient.instance.Log(UIControllerClient.LOGTEXT.HideLayout, displayGameLayout);
        }
        else
        {
            canvasGroup.alpha = 0f;
        }
    }

    private IEnumerator StartToogle(float time)
    {
        yield return new WaitForSeconds(time);
        if (canvasGroup.alpha == 1f)
        {
            // jeżeli alpha 1f oznacza ze layout przyciskow jest wybrany
            toogleActivateObject.OnToogle();
        }
    }

    private void OnEnable()
    {
        OnGameSelected += OnSelectedGame;
    }

    private void OnDisable()
    {
        OnGameSelected -= OnSelectedGame;
    }
}
