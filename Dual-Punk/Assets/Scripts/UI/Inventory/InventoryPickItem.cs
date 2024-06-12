using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class InventoryPickItem : MonoBehaviour 
{
    [SerializeField] private GameObject[] _weaponSlots;
    [SerializeField] private GameObject[] _implantSlots;
    [SerializeField] private GameObject _inventoryItemPrefab;
    private int _equipedSlotIndex => GetComponent<InventoryManager>().EquipedSlotIndex;


    public GameObject ItemPicked(GameObject pickedItem)
    {
        GameObject emptySlot;
        string Tag = pickedItem.tag;

        switch(Tag){
            case "Weapon" :
                emptySlot = FindEmptySlot(_weaponSlots);
                break;
            case "Implant" :
                emptySlot = FindImplantSlot(pickedItem.GetComponent<ImplantScript>());
                break;
            default:
                throw new System.Exception();
        }

        if (emptySlot != null)
        {
            GameObject newItem = Instantiate(_inventoryItemPrefab);

            newItem.GetComponent<InventoryItem>().displayedItem = pickedItem.GetComponent<PickableItem>().itemData;
            newItem.GetComponent<InventoryItem>().displayedItem.prefab = pickedItem;


        }
        return emptySlot;
    }


    //Used to find a slot for a weapon
    public GameObject FindEmptySlot(GameObject[] slots)
    {
        if (_weaponSlots[_equipedSlotIndex].GetComponent<InventorySlots>().heldItem == null)
            return _weaponSlots[_equipedSlotIndex];

        for (int i = 0 ; i < slots.Length; i++)
        {
            if (slots[i].GetComponent<InventorySlots>().heldItem == null)
                return slots[i];
        }
        return null;
    }


    //Find the implant's slot corresponding to the implant type
    public GameObject FindImplantSlot(ImplantScript implantPrefab)
    {
        switch(implantPrefab.Type)
        {
            case ImplantType.Neuralink:
                return _implantSlots[0];
            case ImplantType.ExoSqueleton:
                return _implantSlots[1];
            case ImplantType.Arm:
                return _implantSlots[2];
            case ImplantType.Boots:
                return _implantSlots[3];
            default:
                return null;
        }
    }
}
