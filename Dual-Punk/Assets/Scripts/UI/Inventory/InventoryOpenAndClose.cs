using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryOpenAndClose : MonoBehaviour
{
    public GameObject Inventory;
    bool InventoryIsClose;
    // Start is called before the first frame update
    void Start()
    {
        InventoryIsClose = true;
        Inventory.SetActive(false);
    }

    // Update is called once per frame
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
