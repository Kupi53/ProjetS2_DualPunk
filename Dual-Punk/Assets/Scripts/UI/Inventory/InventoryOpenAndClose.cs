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
                PlayerState.Stop = true;
                Inventory.SetActive(true);
                InventoryIsClose = false;
                GameManager.Instance.InInventory = true;
            }
            else
            {
                PlayerState.Stop = false;
                InventoryIsClose = true;
                GameManager.Instance.InInventory = false;
                Inventory.SetActive(false);
            }
        }
    }
}