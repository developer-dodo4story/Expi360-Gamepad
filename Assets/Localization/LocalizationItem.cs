[System.Serializable]
public class LocalizationItem
{
    public string key;
    public string value;

    public LocalizationItem(string key, string value)
    {
        this.key = key;
        this.value = value;
    }
}