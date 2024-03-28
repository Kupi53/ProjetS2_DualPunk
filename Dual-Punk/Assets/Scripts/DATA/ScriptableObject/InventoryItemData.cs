using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "inventoryItemData", menuName = "ScriptableObject/inventoryItemData")]
public class InventoryItemData : ScriptableObject
{
    public new string name;
    public GameObject prefab;
    public Sprite icon;
    [TextArea(3,10)] 
    public string description;


}
