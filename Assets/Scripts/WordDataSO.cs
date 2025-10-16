using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WordDataSO : ScriptableObject
{
    [SerializeField] private List<WordData> datas = new();
    public List<WordData> Datas => datas;
}
