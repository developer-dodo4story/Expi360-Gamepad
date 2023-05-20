using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string key;

    private Text text;
    private TextMeshPro textMeshPro;
    private TextMeshProUGUI textMeshProUGUI;
    private string additionalText = "";

    private void OnEnable()
    {
        LocalizationManager.OnLocalizationChange += UpdateText;
        UpdateText(); // za kazdym razem kiedy obiekt jest aktywowany nalezy aktualizowac text bo mozliwe ze jak byl nie aktywny jezyk zostal zmieniony, a skoro byl nie katywny nie zostal zaktualizowany
    }

    private void OnDisable()
    {
        LocalizationManager.OnLocalizationChange -= UpdateText;
    }

    /// <summary>
    /// Dodatkowy tekst, nie jest serializowany
    /// </summary>
    /// <param name="text"></param>
    public void AddText(string text)
    {
        additionalText = text;
    }

    // Use this for initialization
    void Start()
    {
        //UpdateText();
    }

    public void UpdateText()
    {
        text = GetComponent<Text>();
        textMeshPro = GetComponent<TextMeshPro>();
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();

        SaveOrLoad(text);
        SaveOrLoad(textMeshPro);
        SaveOrLoad(textMeshProUGUI);
    }

    private void SaveOrLoad(TextMeshPro textMeshPro)
    {
        if (textMeshPro != null)
        {
            if (LocalizationManager.instance.IsThereAKey(key))
            {
                //istnieje, wczytaj
                textMeshPro.text = LocalizationManager.instance.GetLocalizedValue(key) + additionalText;
            }
            else
            {
                //brak klucza zapisz
                string tmpText = textMeshPro.text;
                //tmpText = tmpText.Remove(tmpText.Length);
                //tmpText = tmpText.TrimEnd();
                LocalizationManager.instance.AddLocalizeValue(key, tmpText);
            }
        }
    }

    private void SaveOrLoad(Text text)
    {
        if (text != null)
        {
            if (LocalizationManager.instance.IsThereAKey(key))
            {
                //istnieje, wczytaj
                text.text = LocalizationManager.instance.GetLocalizedValue(key) + additionalText;
            }
            else
            {
                //brak klucza zapisz
                string tmpText = text.text;
                //tmpText = tmpText.Remove(tmpText.Length);
                //tmpText = tmpText.TrimEnd();
                LocalizationManager.instance.AddLocalizeValue(key, tmpText);
            }
        }
    }

    private void SaveOrLoad(TextMeshProUGUI textMeshProUGUI)
    {
        if (textMeshProUGUI != null)
        {
            if (LocalizationManager.instance.IsThereAKey(key))
            {
                //istnieje, wczytaj
                textMeshProUGUI.text = LocalizationManager.instance.GetLocalizedValue(key) + additionalText;
            }
            else
            {
                //brak klucza zapisz
                string tmpText = textMeshProUGUI.text;
                //tmpText = tmpText.Remove(tmpText.Length);
                //tmpText = tmpText.TrimEnd();
                LocalizationManager.instance.AddLocalizeValue(key, tmpText);
            }
        }
    }
}