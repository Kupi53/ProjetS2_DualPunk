using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickItem : MonoBehaviour
{
    [SerializeField] InventoryPickItem inventoryManager;
    private GameObject itemToPick;
    private bool _CanBePicked;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update(){
        if(_CanBePicked && Input.GetButtonDown("Pickup")){
            inventoryManager.ItemPicked(itemToPick);
        }
    }

    void OnTriggerEnter2D(Collider2D collider){
        if(collider.CompareTag("Weapon")){
            _CanBePicked = true;
            itemToPick = collider.gameObject;
    }
}

    void OnTriggerExit2D(Collider2D collider){
        if(collider.CompareTag("Weapon")){
            _CanBePicked = false;
            itemToPick = null;
        }
    }
}
