using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager instance;
    public static Action OnLocalizationChange;

    public string currentFile = "localizedText_en";

    private List<string> keys = new List<string>();
    private List<string> values = new List<string>();

    [Space]
    public bool saveData;
    [Space]
    public bool updateData;
    [Space]
    public bool sort;
    [Tooltip("Use this when manualy add items to list")]
    public bool saveCustomData;

    private Dictionary<string, string> localizedText = new Dictionary<string, string>();
    [SerializeField]
    [Space]
    private LocalizationData localizationData;

    private bool isReady = false;
    private string missingTextString = "Localized text not found";
    private readonly string fileExtension = ".json";

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadLocalizedText(currentFile);
    }

    private string CreatePath(string fileName)
    {
        string filePath = "";
        filePath = Path.Combine(Application.dataPath, "Resources", fileName + fileExtension);

#if UNITY_ANDROID

#endif
#if UNITY_IOS

#endif
#if UNITY_EDITOR

#endif
        return filePath;
    }

    /// <summary>
    /// Wywolana poprzez przycisk
    /// </summary>
    /// <param name="fileName"></param>
    public void LoadLocalizedText(string fileName)
    {
        currentFile = fileName;

        //string filePath = CreatePath(currentFile);

        TextAsset file = Resources.Load(currentFile) as TextAsset;

        if (file != null)
        {
            string dataAsJson = file.ToString();
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);
            keys = new List<string>();
            values = new List<string>();
            localizationData = new LocalizationData();
            localizedText = new Dictionary<string, string>();

            for (int i = 0; i < loadedData.items.Count; i++)
            {
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);

                var localizationItem = new LocalizationItem(loadedData.items[i].key, loadedData.items[i].value);
                localizationData.items.Add(localizationItem);

                keys.Add(loadedData.items[i].key);
                values.Add(loadedData.items[i].value);
            }

            Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries");
            OnLocalizationChange?.Invoke();
        }
        else
        {
            Debug.LogError("Cannot find file!");
        }
        isReady = true;
    }

    public string GetLocalizedValue(string key)
    {
        string result = missingTextString;
        if (localizedText.ContainsKey(key))
        {
            result = localizedText[key];
        }
        return result;
    }

    public void AddLocalizeValue(string key, string value)
    {
        if (localizedText != null)
        {
            if (!localizedText.ContainsKey(key))
            {
                localizedText.Add(key, value);
                keys.Add(key);
                values.Add(value);
            }
        }
    }


#if UNITY_EDITOR
    private void Update()
    {
        if (saveData)
        {
            saveData = false;
            SaveGameData();
        }
        if (updateData)
        {
            updateData = false;
            TransformDictionaryToLocalizationData();
        }
        if (sort)
        {
            sort = false;
            localizationData.Sort();
        }
    }
#endif

    private void SaveGameData()
    {
        string filePath = CreatePath(currentFile);

        if (!saveCustomData)
        {
            TransformDictionaryToLocalizationData();
        }

        if (!string.IsNullOrEmpty(filePath))
        {
            string dataAsJson = JsonUtility.ToJson(localizationData);
            File.WriteAllText(filePath, dataAsJson);
            Debug.Log("Data saved, dictionary contains: " + localizedText.Count + " entries" + ". File name: " + currentFile);
        }
    }

    private void TransformDictionaryToLocalizationData()
    {
        localizationData = new LocalizationData();

        for (int i = 0; i < localizedText.Count; i++)
        {
            var localizationItem = new LocalizationItem(keys[i], values[i]);
            localizationData.items.Add(localizationItem);
        }

        localizationData.Sort();
    }

    public bool IsThereAKey(string key)
    {
        return localizedText.ContainsKey(key);
    }

    public bool GetIsReady()
    {
        return isReady;
    }

}