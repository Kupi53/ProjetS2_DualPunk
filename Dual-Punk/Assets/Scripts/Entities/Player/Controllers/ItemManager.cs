using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using Unity.VisualScripting;
using FishNet.Object;


public class ItemManager : NetworkBehaviour
{
    [SerializeField] 
    private GameObject[] _weaponsSlots;
    
    private GameObject _inventoryManager;
    private PlayerState _playerState;
    private List<GameObject> _items;
    private IImpact _impact;

    #nullable enable
    private GameObject? _item;
    private WeaponScript? _weaponScript;
    #nullable disable

    private int _index;


    private void Start()
    {
        if(!Owner.IsLocalClient) return;

        _inventoryManager = GameObject.FindWithTag("Inventory");
        _index = 0;
        _items = new List<GameObject>();
        _impact = GetComponent<IImpact>();
        _playerState = GetComponent<PlayerState>();

        _inventoryManager.gameObject.SetActive(false);
    }


    private void Update()
    {
        if(!Owner.IsLocalClient) return;

        if (_items.Count > 0)
        {
            if (Input.GetButtonDown("Switch"))
                _index = (_index + 1) % _items.Count;

            _item = _items[_index];

            if (_item.CompareTag("Weapon") && !(_weaponScript = _item.GetComponent<WeaponScript>()).InHand)
            {
                _item.GetComponent<HighlightItem>().Highlight();

                if (Input.GetButtonDown("Pickup") && !_playerState.HoldingWeapon)
                {
                    _index = 0;
                    _inventoryManager.GetComponent<InventoryPickItem>().ItemPicked(_item);
                    UpdateHeldWeapon(_weaponScript);
                }
            }
            else if (_item.CompareTag("Implant")) //Plus verifier que l'implant n'est pas sur une entite
            {
                _item.GetComponent<HighlightItem>().Highlight();

                if (Input.GetButtonDown("Pickup"))
                {
                    _index = 0;
                    //Mettre l'implant dans l'inventaire ou le remplacer avec un autre
                }
            }
            else if (_item.CompareTag("Item"))
            {
                _item.GetComponent<HighlightItem>().Highlight();

                if (Input.GetButtonDown("Pickup"))
                {
                    _index = 0;
                    //Mettre l'item dans l'inventaire
                    _inventoryManager.GetComponent<InventoryPickItem>().ItemPicked(_item);
                }
            }
        }
    }


    void OnTriggerStay2D(Collider2D collider)
    {
        if (!Owner.IsLocalClient) return;

        if (!_items.Contains(collider.gameObject) && (collider.gameObject.CompareTag("Weapon") || collider.gameObject.CompareTag("Implant") || collider.gameObject.CompareTag("Item")))
        {
            _items.Add(collider.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (!Owner.IsLocalClient) return;

        if (_items.Contains(collider.gameObject) && (collider.gameObject.CompareTag("Weapon") || collider.gameObject.CompareTag("Knife") || collider.gameObject.CompareTag("Item")))
        {
            _index = 0;
            _items.Remove(collider.gameObject);
        }
    }

    void UpdateHeldWeapon(WeaponScript weaponScript){
        //Intervetir avec l'arme en main
        _playerState.WeaponScript = weaponScript;
        _playerState.HoldingWeapon = true;
        _weaponScript.gameObject.GetComponent<NetworkObject>().GiveOwnership(base.ClientManager.Connection);
        _weaponScript.PlayerState = _playerState;
        _weaponScript.PlayerRecoil = _impact;
        _weaponScript.InHand = true;
    }
}