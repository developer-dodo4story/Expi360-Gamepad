using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationVersion : MonoBehaviour
{
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetAppVersion();
    }

    private void SetAppVersion()
    {
        if (text != null)
        {
            text.text = "v" + Application.version;
        }
    }
}
