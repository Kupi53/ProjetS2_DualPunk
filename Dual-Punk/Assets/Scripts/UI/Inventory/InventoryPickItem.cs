using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class InventoryPickItem : MonoBehaviour 
{
    [SerializeField] GameObject[] weaponSlots = new GameObject[3];
    [SerializeField] GameObject[] implantSlots = new GameObject [4];
    [SerializeField] GameObject[] consummableSlots = new GameObject[18];
    [SerializeField] GameObject inventoryItemPrefab;

    private int EquipedSlotIndex => GetComponent<InventoryManager>().EquipedSlotIndex;

    public void ItemPicked(GameObject pickedItem)
    {
        GameObject emptySlot = null;
        string Tag = pickedItem.tag;

        switch(Tag){
            case "Consummable" :
                emptySlot = FindEmptySlot(consummableSlots);
                break;
            case "Weapon" :
                emptySlot = FindEmptySlot(weaponSlots);
                break;
            case "Implant" :
                emptySlot = FindImplantSlot(pickedItem.GetComponent<ImplantScript>());
                break;
            default:
                throw new System.Exception();
        }

        if (emptySlot != null)
        {
            GameObject newItem = Instantiate(inventoryItemPrefab);

            //enregistre le prefab de displayed item car displayed item sera detruit juste en dessous
            newItem.GetComponent<InventoryItem>().displayedItem = pickedItem.GetComponent<PickableItem>().itemData;
            newItem.GetComponent<InventoryItem>().displayedItem.prefab = pickedItem;

            newItem.transform.SetParent(emptySlot.transform.parent.parent.GetChild(4));
            newItem.transform.localScale = emptySlot.transform.localScale;
            
            emptySlot.GetComponent<InventorySlots>().SetHeldItem(newItem);

            if (pickedItem.tag == "Weapon") {
                if (weaponSlots[EquipedSlotIndex] != emptySlot) {
                    pickedItem.SetActive(false);
                }
            }
        }
    }

    //Use for each type of slot of the inventory to find if a slot is available.
    public GameObject FindEmptySlot(GameObject[] slots)
    {
        GameObject res = null;

        for (int i = 0 ; i < slots.Length; i++)
        {
            InventorySlots Slot = slots[i].GetComponent<InventorySlots>();

            if (Slot.heldItem == null) {
                res = slots[i];
                break;
            }
        }
        return res;
    }

    public GameObject FindImplantSlot(ImplantScript implantPrefab){
        GameObject res = null;
        switch(implantPrefab.Type){
            case ImplantType.Neuralink:
                res = implantSlots[0];
                break;
            case ImplantType.ExoSqueleton:
                res = implantSlots[1];
                break;
            case ImplantType.Arm:
                res = implantSlots[2];
                break;
            case ImplantType.Boots:
                res = implantSlots[3];
                break;
        }
        return res;
    }
}
