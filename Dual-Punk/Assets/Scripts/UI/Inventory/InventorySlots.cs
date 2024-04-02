using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlots : MonoBehaviour
{

    public GameObject heldItem;

    public void SetHeldItem(GameObject Item){
        heldItem = Item;
        heldItem.transform.position = transform.position;
    }

}
