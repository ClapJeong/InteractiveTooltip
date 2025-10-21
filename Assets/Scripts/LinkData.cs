using UnityEngine;

[System.Serializable]
public class LinkData
{
    [SerializeField] private string key;
    public string Key => key;

    [SerializeField] private string name;
    public string Name => name;

    [SerializeField] private string description;
    public string Description => description;
}
