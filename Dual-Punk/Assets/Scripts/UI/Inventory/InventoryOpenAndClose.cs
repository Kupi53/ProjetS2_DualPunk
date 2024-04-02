using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryOpenAndClose : MonoBehaviour
{
    public GameObject Inventory;
    bool InventoryIsClose;

    void Start()
    {
        InventoryIsClose = true;
    }

    void Update()
    {
        if(Input.GetButtonDown("Inventory")){
            if(InventoryIsClose ){
                Inventory.SetActive(true);
                InventoryIsClose = false;
            }
            else{
                InventoryIsClose = true;
                Inventory.SetActive(false);
            }
        }
    }

}
