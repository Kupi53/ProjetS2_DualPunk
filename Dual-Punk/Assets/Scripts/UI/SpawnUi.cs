using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;


public class SpawnUi : NetworkBehaviour
{
    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject Camera;

    private GameObject LocalUI;
    private GameObject LocalCamera;


    void Start()
    {
      if (Owner.IsLocalClient)
        {
            LocalUI = Instantiate(UI);
            LocalCamera = Instantiate(Camera);

            LocalUI.name = gameObject.name + " UI";
            LocalCamera.name = gameObject.name + " Camera";

            GetComponent<PlayerState>().CameraController = LocalCamera.GetComponent<CameraController>();
            GetComponent<ItemManager>()._inventoryManager = LocalUI.GetComponentInChildren<InventoryManager>().gameObject;
            LocalUI.GetComponentInChildren<InventoryManager>().ItemManager = GetComponent<ItemManager>();
            LocalUI.GetComponentInChildren<InventoryManager>().PlayerState = GetComponent<ItemManager>()._playerState;
            LocalUI.GetComponentInChildren<InventoryManager>().transform.GetChild(0).gameObject.SetActive(false);
            LocalCamera.GetComponent<CameraController>().Player = gameObject;
            LocalUI.GetComponent<LocalPlayerReference>().PlayerState = GetComponent<PlayerState>();
            LocalUI.GetComponent<LocalPlayerReference>().ConsumablesController = GetComponent<ConsumablesController>();

        }
    }
}
