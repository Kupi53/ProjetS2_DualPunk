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
	public HealthManager _healthManager;
    public ImplantController _implantController;
    private List<GameObject> _items;
    private IImpact _impact;
    private int _index;


    private void Start()
    {
        if (!Owner.IsLocalClient) return;

        _index = 0;
        _items = new List<GameObject>();
        _impact = GetComponent<IImpact>();
        _playerState = GetComponent<PlayerState>();
        _healthManager = GetComponent<HealthManager>();
        _implantController = GetComponent<ImplantController>();
    }


    private void Update()
    {
        if (!Owner.IsLocalClient) return;

        if (_items.Count > 0)
        {
            GameObject item = _items[_index];

            if (Input.GetButtonDown("Switch"))
            {
                if (item != null)
                    RemoveHighlight(item);
                _index = (_index + 1) % _items.Count;
            }

            item = _items[_index];
            item.GetComponent<HighlightItem>().Highlight();

            if (!Input.GetButtonDown("Pickup")) return;

            WeaponScript weaponScript;
            ImplantScript implantScript;


            if (item.CompareTag("Weapon") && !(weaponScript = item.GetComponent<WeaponScript>()).InHand)
            {
                _index = 0;
                _items.Remove(item);

                InventorySlots[] weaponsSlots = _inventoryManager.GetComponent<InventoryManager>().WeaponSlots;
                int equippedSlot = _inventoryManager.GetComponent<InventoryManager>().EquipedSlotIndex;
                weaponScript.PlayerState = _playerState;

                if (weaponsSlots[equippedSlot].heldItem == null)
                {
                    UpdateHeldWeapon(weaponScript);
                }

                RemoveHighlight(item);
                _inventoryManager.GetComponent<InventoryPickItem>().ItemPicked(item);
            }

            else if (item.CompareTag("Implant") && !(implantScript = item.GetComponent<ImplantScript>()).IsEquipped) //Plus verifier que l'implant n'est pas sur une entite
            {
                bool pickable = false;

                switch (implantScript.Type)
                {
                    case ImplantType.Neuralink:
                        if (_implantController.NeuralinkImplant == null)
                        {
                            _implantController.NeuralinkImplant = implantScript;
                            pickable = true;
                        }
                        break;

                    case ImplantType.ExoSqueleton:
                        if (_implantController.ExoSqueletonImplant == null)
                        {
                            _implantController.ExoSqueletonImplant = implantScript;
                            pickable = true;
                        }
                        break;

                    case ImplantType.Arm:
                        if (_implantController.ArmImplant == null)
                        {
                            _implantController.ArmImplant = implantScript;
                            pickable = true;
                        }
                        break;

                    case ImplantType.Boots:
                        if (_implantController.BootsImplant == null)
                        {
                            _implantController.BootsImplant = implantScript;
                            pickable = true;
                        }
                        break;
                }

                if (pickable)
                {
                    _index = 0;
                    _items.Remove(item);
                    _inventoryManager.GetComponent<InventoryPickItem>().ItemPicked(item);

                    RemoveHighlight(item);
                    item.GetComponent<SpriteRenderer>().enabled = false;

                    implantScript.IsEquipped = true;
                    implantScript.PlayerState = _playerState;
                }
            }
                
            else if (item.CompareTag("Item"))
            {
                _index = 0;
                _items.Remove(item);
                //Mettre l'item dans l'inventaire
                _inventoryManager.GetComponent<InventoryPickItem>().ItemPicked(item);
            }
        }
    }


    void OnTriggerStay2D(Collider2D collision)
    {
        if (!Owner.IsLocalClient) return;

        GameObject item = collision.gameObject;

        if (!_items.Contains(item))
        {
            if ((item.CompareTag("Weapon") && !item.GetComponent<WeaponScript>().InHand) || item.CompareTag("Implant") && !item.GetComponent<ImplantScript>().IsEquipped || item.CompareTag("Item"))
                _items.Add(item);
        }
    }


    void OnTriggerExit2D(Collider2D collision)
    {
        if (!Owner.IsLocalClient) return;

        GameObject item = collision.gameObject;

        if (item.GetComponent<HighlightItem>() != null) RemoveHighlight(item);

        if (item.CompareTag("Weapon") || item.CompareTag("Item") || item.CompareTag("Implant"))
        {
            _index = 0;
            _items.Remove(item);
        }
    }


    private void RemoveHighlight(GameObject item)
    {
        item.GetComponent<HighlightItem>().Selected = false;
    }

    public void UpdateHeldWeapon(WeaponScript weaponScript)
    {
        //Intervetir avec l'arme en main
        _playerState.WeaponScript = weaponScript;
        _playerState.HoldingWeapon = true;
        GiveOwnershipRPC(weaponScript.gameObject.GetComponent<NetworkObject>(), base.ClientManager.Connection);
        weaponScript.PickUp(_playerState, _impact);
        weaponScript.gameObject.SetActive(true);
    }


    [ServerRpc (RequireOwnership = false)]
    void GiveOwnershipRPC(NetworkObject networkObject, NetworkConnection networkConnection)
    {
        networkObject.GiveOwnership(networkConnection);
    }
}