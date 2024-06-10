using System;
using System.Collections;
using FishNet;
using IO.Swagger.Model;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class InventoryManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image _dropPanel;
    [SerializeField] private Image _inventoryPanel;
    private GameObject _draggedObject;
    private InventorySlots _lastSlotPosition;
    private InventorySlots _currentSlot;
    private ObjectSpawner _objectSpawner;
    private DescriptionManager _descriptionManager;

    public InventorySlots[] WeaponSlots = new InventorySlots[3];
    public PlayerState PlayerState;
    public ItemManager ItemManager;
    public int EquipedSlotIndex;
    public bool swapping;



    void Start()
    {

        _descriptionManager = GetComponent<DescriptionManager>();
        _objectSpawner = GameObject.FindWithTag("ObjectSpawner").GetComponent<ObjectSpawner>();
        EquipedSlotIndex = 0;
        swapping  = false;
        
    }


    void Update()
    {
        //fais suivre l'objet clique par la souris, sur la souris.
        if (_draggedObject != null)
        {
            _draggedObject.transform.position = Input.mousePosition;
        }

        SwapKeybindWeapon();

        var direction = Input.GetAxis("Mouse ScrollWheel");

        if(direction != 0 && !swapping)
        {
            InventorySlots currentWeaponSlot = WeaponSlots[EquipedSlotIndex];

            swapping = true;
            bool found = false;
            int i = 0;
            
            while(i < 3 && !found)
            {
                EquipedSlotIndex = (EquipedSlotIndex + 1)%3;
                if (WeaponSlots[EquipedSlotIndex].heldItem != null)
                    found = true;
                i++;
            }

            InventorySlots nextStoredWeapon = WeaponSlots[EquipedSlotIndex];

            if (nextStoredWeapon != null && nextStoredWeapon != currentWeaponSlot)
            {
                currentWeaponSlot.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                nextStoredWeapon.transform.localScale = new Vector3(2f, 2f, 2f);

                SwapEquipedSlot(currentWeaponSlot, nextStoredWeapon);
            }
        }

        if (swapping)
        { 
            StartCoroutine(SlowScrollWheel());
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {


        if (eventData.button == PointerEventData.InputButton.Left)
        {


            //recupere le raycast du slot clique.
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlots slot = clickedObject.GetComponent<InventorySlots>();

            if (slot != null && slot.heldItem != null && slot.heldItem.GetComponent<InventoryItem>().displayedItem.prefab.tag != "Projectile")
            {

                //change the raycast state needed for the _descriptionManager
                _dropPanel.raycastTarget = true;
                _inventoryPanel.raycastTarget = true;
                _draggedObject = slot.heldItem;
                _lastSlotPosition = slot;
                slot.heldItem = null;

                //Hide descprition panels if the item is dragged
                if (_descriptionManager.isActiveAndEnabled)
                {
                    if (_descriptionManager.GetInCD())
                        _descriptionManager.StopTheCoroutine();

                    else if (_descriptionManager != null){
                        _descriptionManager.GetDescriptionWindow().SetActive(false);
                    }

                }
            }
        }

        //Drop the inventory Item whith right clicking on it 
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlots slot = clickedObject.GetComponent<InventorySlots>();
            _descriptionManager.StopTheCoroutine();
            if (slot.heldItem != null){
                Drop(slot);
            }

        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_draggedObject != null && eventData.button == PointerEventData.InputButton.Left)
        {
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
                _currentSlot = clickedObject.GetComponent<InventorySlots>();

                //if the slot is empty - place item
                if (_currentSlot != null && _currentSlot.heldItem == null)
                {

                    if (Swapable(_currentSlot, _draggedObject))
                    {
                        RefreshScale();
                        PlaceItem();

                        if (_currentSlot == WeaponSlots[EquipedSlotIndex])
                        { 
                            SwapEquipedSlot(_lastSlotPosition, _currentSlot);
                        }

                        else if (_lastSlotPosition == WeaponSlots[EquipedSlotIndex])
                        {
                            GameObject destroyedGameObject = PlayerState.WeaponScript.gameObject;
                            DropWeapon(_lastSlotPosition, null);
                            destroyedGameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        PlaceLastSlotPosition();
                    }

                }

                //if the slot is not empty - swap item
                else if (_currentSlot != null && _currentSlot.heldItem != null)
                {

                    if (Swapable(_currentSlot, _draggedObject) && Swapable(_lastSlotPosition, _currentSlot.heldItem))
                    {
                        RefreshSwapScale();
                        SwapItemPosition();

                        if (_currentSlot == WeaponSlots[EquipedSlotIndex])
                            SwapEquipedSlot(_lastSlotPosition, _currentSlot);
                        else if (_lastSlotPosition == WeaponSlots[EquipedSlotIndex])
                            SwapEquipedSlot(_currentSlot, _lastSlotPosition);
                    }
                    else
                    {
                        PlaceLastSlotPosition();
                    }
                }

                //if there is not slot and no drop - replace item at last pos
                else if (clickedObject.name != "DropItem")
                {
                    PlaceLastSlotPosition();
                }

                //drop the item on the map
                else
                {
                    Drop(_lastSlotPosition);
                }

                _draggedObject = null;
                _dropPanel.raycastTarget = false;
                _inventoryPanel.raycastTarget = false;
            }
        }
    }

    public void SwapEquipedSlot(InventorySlots currentWeaponSlot, InventorySlots nextWeaponSlot)
    {
        GameObject currentStoredObject = currentWeaponSlot.heldItem; 
        GameObject nextStoredObject = nextWeaponSlot.heldItem;

        if (currentStoredObject != null)
        {
            currentStoredObject.transform.localScale = currentWeaponSlot.transform.localScale;
            GameObject destroyedGameObject = PlayerState.WeaponScript.gameObject;
            destroyedGameObject.GetComponent<WeaponScript>().ResetWeapon();
            destroyedGameObject.SetActive(false);
        }

        if (nextStoredObject != null)
        {
            nextStoredObject.transform.localScale = nextWeaponSlot.transform.localScale;
            GameObject equipedObject = nextStoredObject.GetComponent<InventoryItem>().displayedItem.prefab;
            _objectSpawner.SpawnObjectAndUpdateRpc(equipedObject, PlayerState.gameObject.transform.position, Quaternion.identity, InstanceFinder.ClientManager.Connection, ItemManager.gameObject);
            equipedObject.transform.position = PlayerState.gameObject.transform.position;
        }
        else
        {
            PlayerState.HoldingWeapon = false;
        }
    }


//--------------------Auxilaries Functions that work as their name says.------------------------------------

    private bool Swapable(InventorySlots selectedSlot, GameObject selectedItem)
    {
        bool res = false;
        string placedItemName = selectedItem.GetComponent<InventoryItem>().displayedItem.prefab.tag;
        string currentSlotName = selectedSlot.transform.parent.name;

        if (placedItemName == "Weapon" && currentSlotName == "WeaponSlots" || placedItemName == "Implant" &&
            currentSlotName == "ImplantSlot" || placedItemName == "Consummable" && currentSlotName == "ConsumabelSlots")
        {
            res = true;
        }
        
        return res;
    }

    private void SwapItemPosition()
    {
        _lastSlotPosition.GetComponent<InventorySlots>().SetHeldItem(_currentSlot.heldItem);
        _currentSlot.SetHeldItem(_draggedObject);
    }

    private void PlaceItem()
    {
        _lastSlotPosition.heldItem = null;
        _currentSlot.SetHeldItem(_draggedObject);
 
    }

    private void PlaceLastSlotPosition()
    {
        _lastSlotPosition.SetHeldItem(_draggedObject);
    }

    private void RefreshScale()
    {
        Vector3 draggedObjectScale = _draggedObject.transform.localScale;
        Vector3 currentSlotScale = _currentSlot.transform.localScale;

        if (draggedObjectScale != currentSlotScale)
        {
            _draggedObject.transform.localScale = currentSlotScale;
        }
    }

    private void RefreshSwapScale()
    {
        Vector3 draggedObjectScale = _draggedObject.transform.localScale;
        Vector3 currentSlotScale = _currentSlot.transform.localScale;

        if (draggedObjectScale != currentSlotScale)
        {
            _draggedObject.transform.localScale = currentSlotScale;
            _currentSlot.heldItem.transform.localScale = draggedObjectScale;
        }
    }

    private void Drop(InventorySlots slot)
    {
        GameObject destroyedInventoryItem = slot.heldItem;
        GameObject item;

        if (slot.heldItem == null)
        {
            item = _draggedObject.GetComponent<InventoryItem>().displayedItem.prefab;
            destroyedInventoryItem = _draggedObject;
        }
        else
        {
            item = slot.heldItem.GetComponent<InventoryItem>().displayedItem.prefab;
        }

        switch (item.tag)
        {
            case "Weapon":
                DropWeapon(slot, item);
                break;
            case "Implant":
                DropImplant(item);
                break;
            case "Item":
                DropItem(slot);
                break;
            default:
                throw new Exception("pas de tag / mauvais tag");
        }

        slot.heldItem = null;
        Destroy(destroyedInventoryItem);
    }

    private void DropWeapon(InventorySlots slot, GameObject weapon)
    {
        if (slot == WeaponSlots[EquipedSlotIndex])
        {
            PlayerState.WeaponScript.Drop();
            PlayerState.HoldingWeapon = false;
        }
        else
        {
            weapon.SetActive(true);
            weapon.GetComponent<WeaponScript>().Drop();
        }
    }

    private void DropImplant(GameObject implant)
    {
        implant.GetComponent<ImplantScript>().Drop();
        _descriptionManager.UpdateImplantSet();
    }

    private void DropItem(InventorySlots slot){
        
    }

    private void SwapKeybindWeapon()
    {
        if (Input.GetButtonDown("Slot 1") || Input.GetButtonDown("Slot 2") || Input.GetButtonDown("Slot 3"))
        {
            InventorySlots currentWeaponSlot = WeaponSlots[EquipedSlotIndex];

            if (Input.GetButtonDown("Slot 1"))
                EquipedSlotIndex = 0;
            else if (Input.GetButtonDown("Slot 2"))
                EquipedSlotIndex = 1;
            else if(Input.GetButtonDown("Slot 3"))
                EquipedSlotIndex = 2;

            InventorySlots nextStoredWeapon = WeaponSlots[EquipedSlotIndex];

            if (currentWeaponSlot != nextStoredWeapon)
            {
                currentWeaponSlot.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                nextStoredWeapon.transform.localScale = new Vector3(2f, 2f, 2f);

                SwapEquipedSlot(currentWeaponSlot, nextStoredWeapon);
            }
        }
    }

    private IEnumerator SlowScrollWheel()
    {
        yield return new WaitForSeconds(1f);
        swapping = false;
    }
}
