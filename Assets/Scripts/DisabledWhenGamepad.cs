using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisabledWhenGamepad : MonoBehaviour
{
    private void OnEnable()
    {
        ToogleActivateObject.OnGamePadEnabled += OnGamepadEnabled;
        ToogleActivateObject.OnGamePadDisabled += OnGamepadDisabled;
    }

    private void OnDisable()
    {
        //ToogleActivateObject.OnGamePadEnabled -= OnGamepadEnabled;
        //ToogleActivateObject.OnGamePadDisabled -= OnGamepadDisabled;
    }

    public void OnGamepadEnabled()
    {
        gameObject.SetActive(false);
    }

    public void OnGamepadDisabled()
    {
        gameObject.SetActive(true);
    }
}
