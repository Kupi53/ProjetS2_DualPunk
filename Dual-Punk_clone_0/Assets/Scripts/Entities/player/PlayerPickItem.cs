using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class PlayerPickItem : NetworkBehaviour
{
    private InventoryPickItem inventoryManager;
    private GameObject itemToPick;
    private bool _CanBePicked;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!Owner.IsLocalClient) return;
        inventoryManager = GameObject.FindWithTag("Inventory").GetComponent<InventoryPickItem>();
    }

    void Update(){
        if (!Owner.IsLocalClient) return;
        if(_CanBePicked && Input.GetButtonDown("Pickup")){
            inventoryManager.ItemPicked(itemToPick);
        }
    }

    void OnTriggerEnter2D(Collider2D collider){
        if (!Owner.IsLocalClient) return;
        if(collider.CompareTag("Weapon")){
            _CanBePicked = true;
            itemToPick = collider.gameObject;
    }
}

    void OnTriggerExit2D(Collider2D collider){
        if (!Owner.IsLocalClient) return;
        if(collider.CompareTag("Weapon")){
            _CanBePicked = false;
            itemToPick = null;
        }
    }
}
