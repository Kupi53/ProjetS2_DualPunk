using UnityEngine;


public class InventorySlots : MonoBehaviour
{
    public GameObject heldItem;

    public void SetHeldItem(GameObject Item){
        heldItem = Item;
        heldItem.transform.position = transform.position;
    }

}
