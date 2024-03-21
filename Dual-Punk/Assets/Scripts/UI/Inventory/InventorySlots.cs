using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlots : MonoBehaviour
{

    public GameObject HeldItem;


    public void SetHeldItem(GameObject Item){
        HeldItem = Item;
        HeldItem.transform.position = this.transform.position;
    }

}
