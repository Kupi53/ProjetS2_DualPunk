using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class InventoryOpenAndClose : MonoBehaviour
{
    public PlayerState PlayerState { get; set; }
    public GameObject Inventory;
    bool InventoryIsClose;

    void Start()
    {
        InventoryIsClose = true;
        PlayerState = gameObject.transform.parent.parent.gameObject.GetComponent<LocalPlayerReference>().PlayerState;
    }

    void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            if (InventoryIsClose)
            {
                if (PlayerState.WeaponScript != null)
                    PlayerState.WeaponScript.enabled = false;
                Inventory.SetActive(true);
                InventoryIsClose = false;
            }
            else
            {
                if (PlayerState.WeaponScript != null)
                    PlayerState.WeaponScript.enabled = true;
                InventoryIsClose = true;
                Inventory.SetActive(false);
            }
        }
    }
}