using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using Unity.VisualScripting;
using FishNet.Object;
using FishNet.Connection;


public class ItemManager : NetworkBehaviour
{
    public GameObject _inventoryManager;
    public PlayerState _playerState;
    private List<GameObject> _items;
    private IImpact _impact;
    private int _index;



    private void Start()
    {
        if(!Owner.IsLocalClient) return;
        _index = 0;
        _items = new List<GameObject>();
        _impact = GetComponent<IImpact>();
        _playerState = GetComponent<PlayerState>();
        
    }


    private void Update()
    {
        if(!Owner.IsLocalClient) return;

        if (_items.Count > 0)
        {
            GameObject item = _items[_index];

            if (Input.GetButtonDown("Switch")){
                if(item != null) item.GetComponent<HighlightItem>().selectedWeapon = false;
                _index = (_index + 1) % _items.Count;
            }

            item = _items[_index];
            WeaponScript weaponScript;
            item.GetComponent<HighlightItem>().Highlight();

            if (item.CompareTag("Weapon") && !(weaponScript = item.GetComponent<WeaponScript>()).InHand)
            {


                if (Input.GetButtonDown("Pickup"))
                {
                    _index = 0;
                    _items.Remove(item);

                    InventorySlots[] weaponsSlots = _inventoryManager.GetComponent<InventoryManager>().WeaponSlots;
                    int equippedSlot = _inventoryManager.GetComponent<InventoryManager>().EquipedSlotIndex;
                    if(weaponsSlots[equippedSlot].heldItem == null){
                        UpdateHeldWeapon(weaponScript);
                    }

                    _inventoryManager.GetComponent<InventoryPickItem>().ItemPicked(item);
                    
                }
            }
            else if (item.CompareTag("Implant")) //Plus verifier que l'implant n'est pas sur une entite
            {
                item.GetComponent<HighlightItem>().Highlight();

                if (Input.GetButtonDown("Pickup"))
                {
                    _index = 0;
                    _items.Remove(item);
                    //Mettre l'implant dans l'inventaire ou le remplacer avec un autre
                }
            }
            else if (item.CompareTag("Item"))
            {
                item.GetComponent<HighlightItem>().Highlight();

                if (Input.GetButtonDown("Pickup"))
                {
                    _index = 0;
                    _items.Remove(item);
                    //Mettre l'item dans l'inventaire
                    _inventoryManager.GetComponent<InventoryPickItem>().ItemPicked(item);
                }
            }
        }
    }


    void OnTriggerStay2D(Collider2D collision)
    {
        if (!Owner.IsLocalClient) return;

        GameObject item = collision.gameObject;

        if (!_items.Contains(item))
        {
            if ((item.CompareTag("Weapon") && !item.GetComponent<WeaponScript>().InHand) || item.CompareTag("Implant") || item.CompareTag("Item"))
                _items.Add(item);
        }
    }


    void OnTriggerExit2D(Collider2D collision)
    {
        if (!Owner.IsLocalClient) return;

        GameObject item = collision.gameObject;
        if(item.GetComponent<HighlightItem>() != null) item.GetComponent<HighlightItem>().selectedWeapon = false;

        if (item.CompareTag("Weapon") || item.CompareTag("Knife") || item.CompareTag("Item"))
        {
            _index = 0;
            _items.Remove(item);
        }
    }


    public void UpdateHeldWeapon(WeaponScript weaponScript){
        //Intervetir avec l'arme en main
        _playerState.WeaponScript = weaponScript;
        _playerState.HoldingWeapon = true;
        GiveOwnershipRPC(weaponScript.gameObject.GetComponent<NetworkObject>(), base.ClientManager.Connection);
        weaponScript.PlayerState = _playerState;
        weaponScript.PlayerRecoil = _impact;
        weaponScript.InHand = true;
    }

    [ServerRpc (RequireOwnership = false)]
    void GiveOwnershipRPC(NetworkObject networkObject, NetworkConnection networkConnection){
        Debug.Log("gave ownership");
        networkObject.GiveOwnership(networkConnection);
    }
}