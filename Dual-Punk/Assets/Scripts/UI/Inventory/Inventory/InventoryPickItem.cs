using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPickItem : MonoBehaviour
{
    [SerializeField] GameObject[] Slots = new GameObject[30];
    [SerializeField] GameObject inventoryItemPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ItemPicked(GameObject pickedItem){
        GameObject emptySlot = null;
        for (int i = 0 ; i < Slots.Length; i++){

            InventorySlots Slot = Slots[i].GetComponent<InventorySlots>();

            if (Slot.HeldItem == null){
                emptySlot = Slots[i];
                break;
            }
        }

        if(emptySlot != null){

            GameObject newItem = Instantiate(inventoryItemPrefab);
            RectTransform parentPosition = emptySlot.GetComponent<RectTransform>();
            Debug.Log(newItem);
            newItem.GetComponent<InventoryItem>().displayedItem = pickedItem;
            newItem.transform.SetParent(parentPosition);

            emptySlot.GetComponent<InventorySlots>().SetHeldItem(newItem);

        }
    }
}
