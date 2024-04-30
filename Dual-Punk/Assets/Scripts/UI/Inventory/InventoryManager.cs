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
    public PlayerState PlayerState {get; set;}
    public InventorySlots[] WeaponSlots = new InventorySlots[3];
    public ItemManager ItemManager {get; set;}
    private GameObject draggedObject;
    private InventorySlots LastSlotPosition;
    private InventorySlots currentSlot;
    private ObjectSpawner objectSpawner;
    private DescriptionManager descriptionManager;
    [SerializeField] private Image DropPanel;
    [SerializeField] private Image inventoryPanel;
    public int EquipedSlotIndex;
    public bool selectedSlotActiveness;
    public bool swapping;


    void Start(){

        descriptionManager = GetComponent<DescriptionManager>();
        selectedSlotActiveness = GetComponent<selectedSlotManager>().GetActiveness();
        objectSpawner = GameObject.Find("ObjectSpawner").GetComponent<ObjectSpawner>();
        EquipedSlotIndex = 0;
        swapping  = false;
        
    }
    void Update()
    {
        //fais suivre l'objet clique par la souris, sur la souris.
        if(draggedObject != null){
            draggedObject.transform.position = Input.mousePosition;
        }

        SwapKeybindWeapon();

        var direction = Input.GetAxis("Mouse ScrollWheel");
        if(direction != 0 && !swapping){

            swapping = true;
            InventorySlots currentWeaponSlot = WeaponSlots[EquipedSlotIndex];
            bool found = false;
            int i = 0;
            
            while(i < 3 && !found){
                EquipedSlotIndex = (EquipedSlotIndex + 1)%3;
                i++;
                if(WeaponSlots[EquipedSlotIndex].heldItem != null) found = true;
            }
            InventorySlots nextStoredWeapon = WeaponSlots[EquipedSlotIndex];

            if(nextStoredWeapon != null && nextStoredWeapon != currentWeaponSlot) {
                currentWeaponSlot.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                nextStoredWeapon.transform.localScale = new Vector3(2f, 2f, 2f);

                SwapEquipedSlot(currentWeaponSlot, nextStoredWeapon);


            }
        }
        if(swapping) StartCoroutine(SlowScrollWheel());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //change the raycast state needed for the descriptionManager
        DropPanel.raycastTarget = true;
        inventoryPanel.raycastTarget = true;

        if(eventData.button == PointerEventData.InputButton.Left){

            //recupere le raycast du slot clique.
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlots slot = clickedObject.GetComponent<InventorySlots>();

            if (slot != null && slot.heldItem != null){
                draggedObject = slot.heldItem;
                slot.heldItem = null;
                LastSlotPosition = slot;


                //Hide descprition panels if the item is dragged
                if(descriptionManager.isActiveAndEnabled){
                    if(descriptionManager.GetInCD()) descriptionManager.StopTheCoroutine();
                    else descriptionManager.GetDescriptionWindow().SetActive(false);
                }
            }
        }
        //Drop the inventory Item whith right clicking on it 
        if(eventData.button == PointerEventData.InputButton.Right){
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlots slot = clickedObject.GetComponent<InventorySlots>();

            if(slot != null && slot.heldItem != null) {

                if(slot == WeaponSlots[EquipedSlotIndex]){
                    DropWeapon();
                }
                else{
                    SpawnInventoryItem(slot.heldItem);
                }

                Destroy(slot.heldItem);
                slot.heldItem = null;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(draggedObject != null && eventData.button == PointerEventData.InputButton.Left){
            
            if (eventData.pointerCurrentRaycast.gameObject != null){

                GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
                currentSlot = clickedObject.GetComponent<InventorySlots>();

                //if the slot is empty - place item
                if(currentSlot != null && currentSlot.heldItem == null){
                    if(Swapable(currentSlot, draggedObject)){
                        RefreshScale();
                        PlaceItem();
                        if(currentSlot == WeaponSlots[EquipedSlotIndex]) SwapEquipedSlot(LastSlotPosition, currentSlot);
                        if(LastSlotPosition == WeaponSlots[EquipedSlotIndex]){
                            GameObject destroyedGameObject = PlayerState.WeaponScript.gameObject;
                            DropWeapon();
                            destroyedGameObject.SetActive(false);
                        }
                    }
                    else{
                        PlaceLastSlotPosition();
                    }

                }
                //if the slot is not empty - swap item
                else if (currentSlot != null && currentSlot.heldItem != null){
                    if(Swapable(currentSlot, draggedObject) && Swapable(LastSlotPosition, currentSlot.heldItem)){
                        RefreshSwapScale();
                        SwapItemPosition();
                        if(currentSlot == WeaponSlots[EquipedSlotIndex]) SwapEquipedSlot(LastSlotPosition, currentSlot);
                        else if(LastSlotPosition == WeaponSlots[EquipedSlotIndex]) SwapEquipedSlot(currentSlot, LastSlotPosition);
                    }
                    else{
                        PlaceLastSlotPosition();
                    }
                }

                //if there is not slot and no drop - replace item at last pos
                else if (clickedObject.name != "DropItem"){
                    PlaceLastSlotPosition();
                }

                //drop the item on the map
                else{
                    if(LastSlotPosition == WeaponSlots[EquipedSlotIndex]){
                        PlayerState.WeaponScript.Drop();
                        PlayerState.HoldingWeapon = false;
                        PlayerState.PointerScript.SpriteNumber = 0;
                    }
                    else{
                        SpawnInventoryItem(draggedObject);
                    }

                    Destroy(draggedObject);


                }

                draggedObject = null;
                DropPanel.raycastTarget = false;
                inventoryPanel.raycastTarget = false;
            }
        }
    }

    public void SwapEquipedSlot(InventorySlots currentWeaponSlot, InventorySlots nextWeaponSlot){

        GameObject currentStoredObject = currentWeaponSlot.heldItem; 
        GameObject nextStoredObject = nextWeaponSlot.heldItem;

        if(nextStoredObject != null){

            nextStoredObject.transform.localScale = nextWeaponSlot.transform.localScale;
            GameObject equipedObject = nextStoredObject.GetComponent<InventoryItem>().displayedItem.prefab;
            objectSpawner.SpawnObjectAndUpdateRpc(equipedObject, PlayerState.gameObject.transform.position, new Quaternion(), InstanceFinder.ClientManager.Connection, ItemManager.gameObject);
            equipedObject.transform.position = PlayerState.gameObject.transform.position;
            
        }

        if(currentStoredObject != null){

            currentStoredObject.transform.localScale = currentWeaponSlot.transform.localScale;
            GameObject destroyedGameObject = PlayerState.WeaponScript.gameObject;
            destroyedGameObject.GetComponent<WeaponScript>().Drop();
            destroyedGameObject.SetActive(false);

        }

    }

//--------------------Auxilaries Functions that work as their name says.------------------------------------

    private bool Swapable(InventorySlots selectedSlot, GameObject selectedItem){
        bool res = false;
        string placedItemName = selectedItem.GetComponent<InventoryItem>().displayedItem.prefab.tag;
        string currentSlotName = selectedSlot.transform.parent.name;

        if(currentSlotName == "InventorySlots") res = true;
        else{
            if(placedItemName == "Weapon" && currentSlotName == "WeaponSlots") res = true;
            else if(placedItemName == "Implant" && currentSlotName == "ImplantSlot") res = true;
            else if(placedItemName == "Item" && currentSlotName == "ConsumabelSlots") res = true;
        }
        return res;
    }

    private void SwapItemPosition(){
        if(currentSlot == WeaponSlots[EquipedSlotIndex]){
            
        }
        LastSlotPosition.GetComponent<InventorySlots>().SetHeldItem(currentSlot.heldItem);
        currentSlot.SetHeldItem(draggedObject);
    }

    private void PlaceItem(){
        currentSlot.SetHeldItem(draggedObject);
        LastSlotPosition.heldItem = null;
    }

    private void PlaceLastSlotPosition(){
        LastSlotPosition.SetHeldItem(draggedObject);
    }

    private void RefreshScale(){
        Vector3 draggedObjectScale = draggedObject.transform.localScale;
        Vector3 currentSlotScale = currentSlot.transform.localScale;

        if(draggedObjectScale != currentSlotScale){
            draggedObject.transform.localScale = currentSlotScale;
        }
    }

    private void RefreshSwapScale(){
        Vector3 draggedObjectScale = draggedObject.transform.localScale;
        Vector3 currentSlotScale = currentSlot.transform.localScale;

        if(draggedObjectScale != currentSlotScale){
            draggedObject.transform.localScale = currentSlotScale;
            currentSlot.heldItem.transform.localScale = draggedObjectScale;
        }
    }

    private void SpawnInventoryItem(GameObject spawnedItem){
        GameObject spawnedItemPrefab = spawnedItem.GetComponent<InventoryItem>().displayedItem.prefab;
        Vector3 spawnPosition = GameObject.FindWithTag("Player").transform.position;
        objectSpawner.SpawnWeapons(spawnedItemPrefab, spawnPosition, Quaternion.identity);
    }

    private void DropWeapon(){
        PlayerState.WeaponScript.Drop();
        PlayerState.HoldingWeapon = false;
        PlayerState.PointerScript.SpriteNumber = 0;
    }

    private void SwapKeybindWeapon(){

        if(Input.GetButtonDown("Slot 1") || Input.GetButtonDown("Slot 2") || Input.GetButtonDown("Slot 3"))
        {
            InventorySlots currentWeaponSlot = WeaponSlots[EquipedSlotIndex];
            if(Input.GetButtonDown("Slot 1")) {
                EquipedSlotIndex = 0;

            }
            else if(Input.GetButtonDown("Slot 2")) EquipedSlotIndex = 1;
            else if(Input.GetButtonDown("Slot 3")) EquipedSlotIndex = 2;

            InventorySlots nextStoredWeapon = WeaponSlots[EquipedSlotIndex];

            if(currentWeaponSlot != nextStoredWeapon){

                currentWeaponSlot.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                nextStoredWeapon.transform.localScale = new Vector3(2f, 2f, 2f);

                SwapEquipedSlot(currentWeaponSlot, nextStoredWeapon);
            }
        }
    }

    private IEnumerator SlowScrollWheel(){
        yield return new WaitForSeconds(1f);
        swapping = false;
    }


}
