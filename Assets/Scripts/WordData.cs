[System.Serializable]
public class WordData
{
    public string key;
    public string description;

    public WordData(string key, string description)
    {
        this.key = key;
        this.description = description;
    }
}
