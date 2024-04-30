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
        if(!Owner.IsLocalClient) return;

        _index = 0;
        _items = new List<GameObject>();
        _impact = GetComponent<IImpact>();
        _playerState = GetComponent<PlayerState>();
        _healthManager = GetComponent<HealthManager>();
        _implantController = GetComponent<ImplantController>();
    }


    private void Update()
    {
        if(!Owner.IsLocalClient) return;

        if (_items.Count > 0)
        {
            GameObject item = _items[_index];

            if (Input.GetButtonDown("Switch")){
                if(item != null) item.GetComponent<HighlightItem>().selected = false;
                _index = (_index + 1) % _items.Count;
            }

            item = _items[_index];
            WeaponScript weaponScript;
            ImplantScript implantScript;
            item.GetComponent<HighlightItem>().Highlight();

            if (item.CompareTag("Weapon") && !(weaponScript = item.GetComponent<WeaponScript>()).InHand)
            {
                if (Input.GetButtonDown("Pickup"))
                {
                    _index = 0;
                    _items.Remove(item);

                    InventorySlots[] weaponsSlots = _inventoryManager.GetComponent<InventoryManager>().WeaponSlots;
                    int equippedSlot = _inventoryManager.GetComponent<InventoryManager>().EquipedSlotIndex;
                    
                    if(weaponsSlots[equippedSlot].heldItem == null)
                    {
                        UpdateHeldWeapon(weaponScript);
                    }
                    
                    item.GetComponent<HighlightItem>().selected = false;
                    _inventoryManager.GetComponent<InventoryPickItem>().ItemPicked(item);
                    
                }
            }
            else if (item.CompareTag("Implant") && !(implantScript = item.GetComponent<ImplantScript>()).IsEquipped) //Plus verifier que l'implant n'est pas sur une entite
            {
                if (Input.GetButtonDown("Pickup"))
                {
                    _index = 0;
                    _items.Remove(item);

                    item.GetComponent<HighlightItem>().selected = false;
                    _inventoryManager.GetComponent<InventoryPickItem>().ItemPicked(item);
                    item.GetComponent<SpriteRenderer>().enabled = false;

                    ImplantScript _implantScript = item.GetComponent<ImplantScript>();
                    switch (_implantScript.Type){
                        case ImplantType.Neuralink:
                            _implantController.NeuralinkImplant = _implantScript;
                            break;
                        case ImplantType.ExoSqueleton:
                            _implantController.ExoSqueletonImplant = _implantScript;
                            break;
                        case ImplantType.Arm:
                            _implantController.ArmImplant = _implantScript;
                            break;
                        case ImplantType.Boots:
                            _implantController.BootsImplant = _implantScript;
                            break;
                        default:
                            throw new Exception();
                    }
                    _implantScript.IsEquipped = true;
                    _implantScript.PlayerState = _playerState;
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
            if ((item.CompareTag("Weapon") && !item.GetComponent<WeaponScript>().InHand) || item.CompareTag("Implant") && !item.GetComponent<ImplantScript>().IsEquipped || item.CompareTag("Item"))
                _items.Add(item);
        }
    }


    void OnTriggerExit2D(Collider2D collision)
    {
        if (!Owner.IsLocalClient) return;

        Debug.Log("lkjasdf");

        GameObject item = collision.gameObject;
        if(item.GetComponent<HighlightItem>() != null) item.GetComponent<HighlightItem>().selected = false;

        if (item.CompareTag("Weapon") || item.CompareTag("Knife") || item.CompareTag("Item") || item.CompareTag("Implant"))
        {
            _index = 0;
            _items.Remove(item);
        }
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