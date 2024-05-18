using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemIds", order = 1)]
public class ItemIds : ScriptableObject
{
    public static ItemIds Instance 
    {
        get 
        {
            return Resources.Load<ItemIds>("ItemIds");
        }
    }
    [SerializeField] public GameObject[] _itemPrefabs = new GameObject[12];
    [SerializeField] public Dictionary<string, GameObject> IdTable {
        get
        {
            return new Dictionary<string, GameObject>
            {
                // weapons
                { "0001", _itemPrefabs[0] },
                { "0002", _itemPrefabs[1] },
                { "0003", _itemPrefabs[2] },
                { "0004", _itemPrefabs[3] },
                { "0005", _itemPrefabs[4] },
                { "0006", _itemPrefabs[6] },
                { "0007", _itemPrefabs[7] },
                { "0008", _itemPrefabs[8] },

                // items (consumables)

                //implants
                { "0009", _itemPrefabs[9] },


                // nathan piou piou (troll)
                { "9999", _itemPrefabs[5] },
            };
        }
    }
}
