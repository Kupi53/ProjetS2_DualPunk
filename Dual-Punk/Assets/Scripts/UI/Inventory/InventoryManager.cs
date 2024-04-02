using System.Collections;
using System.Collections.Generic;
using IngameDebugConsole;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject draggedObject;
    private InventorySlots LastSlotPosition;
    private InventorySlots currentSlot;
    private GameObject selectedSlotIcon;
    DescriptionManager descriptionManager => GetComponent<DescriptionManager>();
    [SerializeField] private Image DropPanel;
    [SerializeField] private Image inventoryPanel;

    void Update()
    {
        //fais suivre l'objet clique par la souris, sur la souris.
        if(draggedObject != null){
            draggedObject.transform.position = Input.mousePosition;
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //change the raycast state needed for the descriptionManager
        DropPanel.raycastTarget = true;
        inventoryPanel.raycastTarget = true;

        //Hide selected icon when dragging object
        if(selectedSlotIcon != null) selectedSlotIcon.SetActive(false);

        //Hide descprition panels if the item is dragged
        if(descriptionManager.GetInCD()) descriptionManager.StopTheCoroutine();
        else descriptionManager.GetDescriptionWindow().SetActive(false);

        if(eventData.button == PointerEventData.InputButton.Left){

            //recupere le raycast du slot clique.
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlots slot = clickedObject.GetComponent<InventorySlots>();

            if (slot != null && slot.heldItem != null){
                draggedObject = slot.heldItem;
                slot.heldItem = null;
                LastSlotPosition = slot;
            }
        }

        if(eventData.button == PointerEventData.InputButton.Right){
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlots slot = clickedObject.GetComponent<InventorySlots>();

            if(slot != null && slot.heldItem != null) {
                SpawnInventoryItem(slot, slot.heldItem);
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
                    Debug.Log("Test du drop");
                    SpawnInventoryItem(LastSlotPosition, draggedObject);
                }

                draggedObject = null;
                DropPanel.raycastTarget = false;
                inventoryPanel.raycastTarget = false;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData){
        if(eventData.pointerCurrentRaycast.gameObject.tag == "InventorySlot"){
            selectedSlotIcon = eventData.pointerCurrentRaycast.gameObject.transform.GetChild(0).gameObject;
            selectedSlotIcon.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData){
        if(selectedSlotIcon != null){
            selectedSlotIcon.SetActive(false);
            selectedSlotIcon = null;
        }
    }

    //Auxilaries Functions that work as their name says.

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
        LastSlotPosition.GetComponent<InventorySlots>().SetHeldItem(currentSlot.heldItem);
        currentSlot.SetHeldItem(draggedObject);
    }

    private void PlaceItem(){
        currentSlot.SetHeldItem(draggedObject);
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

    private void SpawnInventoryItem(InventorySlots selectedSlot, GameObject spawnedItem){
        Vector3 spawnPosition = GameObject.FindWithTag("Player").transform.position;
        Instantiate(spawnedItem.GetComponent<InventoryItem>().displayedItem.prefab, spawnPosition, new Quaternion());

        selectedSlot.GetComponent<InventorySlots>().heldItem = null;
        Destroy(spawnedItem);
    }

}
