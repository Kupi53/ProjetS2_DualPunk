using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPickItem : MonoBehaviour
{
    [SerializeField] GameObject[] Slots = new GameObject[30];
    [SerializeField] GameObject inventoryItemPrefab;

    public void ItemPicked(GameObject pickedItem){
        GameObject emptySlot = null;
        for (int i = 0 ; i < Slots.Length; i++){

            InventorySlots Slot = Slots[i].GetComponent<InventorySlots>();

            if (Slot.heldItem == null){
                emptySlot = Slots[i];
                break;
            }
        }

        if(emptySlot != null){

            GameObject newItem = Instantiate(inventoryItemPrefab);

            //enregistre le prefab de displayed item car displayed item sera detruit juste en dessous
            newItem.GetComponent<InventoryItem>().displayedItem = pickedItem.GetComponent<PickableItem>().itemData;

            newItem.transform.SetParent(emptySlot.transform.parent.parent.GetChild(1));
            newItem.transform.localScale = emptySlot.transform.localScale;
            
            emptySlot.GetComponent<InventorySlots>().SetHeldItem(newItem);
            Destroy(pickedItem);
        }
    }
}
