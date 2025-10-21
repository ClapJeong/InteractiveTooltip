using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LinkDataSO : ScriptableObject
{
    [SerializeField] private List<LinkData> linkDatas;
    public List<LinkData> LinkDatas => linkDatas;

    private readonly Dictionary<string, LinkData> searchMap = new();

    public bool TryGetData(string key, out LinkData data)
    {
        if (searchMap.TryGetValue(key, out data))
            return true;

        foreach (var linkData in linkDatas)
        {
            if (linkData.Key == key)
            {
                searchMap[key] = linkData;
                data = linkData;
                return true;
            }
        }

        return false;
    }
}
