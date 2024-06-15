
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/ItemIds", order = 1)]
public class ItemIds : ScriptableObject
{
    public static ItemIds Instance 
    {
        get 
        {
            return Resources.Load<ItemIds>("ItemIds");
        }
    }
    [SerializeField] public List<GameObject> _itemPrefabs;
    [SerializeField] public Dictionary<string, GameObject> IdTable {
        get
        {
            Dictionary<string, GameObject> table = new Dictionary<string, GameObject>();
            for (int i = 0; i < _itemPrefabs.Count; i++)
            {
                table[NumberToId(i)] = _itemPrefabs[i];
            }
            return table;
        }
    }

    static string NumberToId(int num)
    {
        if (num < 0 || num > 9999)
        {
            throw new System.Exception("");
        }
        else
        {
            return num.ToString().PadLeft(4, '0');
        }
    }
}
