using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryPickItem : MonoBehaviour
{
    [SerializeField] GameObject[] weaponSlots = new GameObject[3];
    [SerializeField] GameObject[] implantSlots = new GameObject [4];
    [SerializeField] GameObject[] consummableSlots = new GameObject[3];
    [SerializeField] GameObject[] inventorySlots = new GameObject[15];
    [SerializeField] GameObject inventoryItemPrefab;

    private int EquipedSlotIndex => GetComponent<InventoryManager>().EquipedSlotIndex;

    public void ItemPicked(GameObject pickedItem){
        GameObject emptySlot = null;
        string Tag = pickedItem.tag;

        switch(Tag){
            case "Item" :
                emptySlot = FindEmptySlot(consummableSlots);
                break;
            case "Weapon" :
                emptySlot = FindEmptySlot(weaponSlots);
                break;
            default :
                emptySlot = FindEmptySlot(implantSlots);
                break;
        }

        if(emptySlot == null) emptySlot = FindEmptySlot(inventorySlots);

        if(emptySlot != null){
            GameObject newItem = Instantiate(inventoryItemPrefab);

            //enregistre le prefab de displayed item car displayed item sera detruit juste en dessous
            newItem.GetComponent<InventoryItem>().displayedItem = pickedItem.GetComponent<PickableItem>().itemData;

            newItem.transform.SetParent(emptySlot.transform.parent.parent.GetChild(5));
            newItem.transform.localScale = emptySlot.transform.localScale;
            
            emptySlot.GetComponent<InventorySlots>().SetHeldItem(newItem);

            if(weaponSlots[EquipedSlotIndex] != emptySlot){
                Destroy(pickedItem);
            }

        }
    }

    //Use for each type of slot of the inventory to find if a slot is available.
    public GameObject FindEmptySlot(GameObject[] slots){
        GameObject res = null;
        for (int i = 0 ; i < slots.Length; i++){

            InventorySlots Slot = slots[i].GetComponent<InventorySlots>();

            if (Slot.heldItem == null){
                res = slots[i];
                break;
            }
        }
        return res;
    }
}
