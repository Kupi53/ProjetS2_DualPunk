using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickItem : MonoBehaviour
{
    public GameObject inventoryManager;
    private GameObject itemToPick;
    private bool _CanBePicked;
    
    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = GameObject.FindWithTag("Inventory");
        inventoryManager.gameObject.SetActive(false);
    }

    void Update(){
        if(_CanBePicked && Input.GetButtonDown("Pickup")){
            //inventoryManager.GetComponent<InventoryPickItem>().ItemPicked(itemToPick);
        }
    }

    void OnTriggerEnter2D(Collider2D collider){
        if(collider.CompareTag("Weapon") || collider.CompareTag("Item") || collider.CompareTag("Implant")){
            _CanBePicked = true;
            itemToPick = collider.gameObject;
    }
}

    void OnTriggerExit2D(Collider2D collider){
        if(collider.CompareTag("Weapon") || collider.CompareTag("Item") || collider.CompareTag("Implant")){
            _CanBePicked = false;
            itemToPick = null;
        }
    }
}
