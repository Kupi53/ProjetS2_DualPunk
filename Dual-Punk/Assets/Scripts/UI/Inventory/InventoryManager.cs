using System.Collections;
using System.Collections.Generic;
using IngameDebugConsole;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    GameObject draggedObject;
    GameObject LastSlotPosition;

    void Update()
    {
        //fais suivre l'objet clique par la souris, sur la souris.
        if(draggedObject != null){
            draggedObject.transform.position = Input.mousePosition;
        }
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log(eventData.pointerCurrentRaycast,gameObject);
        if(eventData.button == PointerEventData.InputButton.Left){

            //recupere le raycast du slot clique.
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlots slot = clickedObject.GetComponent<InventorySlots>();

            if (slot != null && slot.heldItem != null){
                draggedObject = slot.heldItem;
                slot.heldItem = null;
                LastSlotPosition = clickedObject;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log(draggedObject);

        if(draggedObject != null && eventData.button == PointerEventData.InputButton.Left){
            
            if (eventData.pointerCurrentRaycast.gameObject != null){

                GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
                InventorySlots slot = clickedObject.GetComponent<InventorySlots>();

                //if the slot is empty - place item
                if(slot != null && slot.heldItem == null){

                    slot.SetHeldItem(draggedObject);
                    draggedObject = null;

                }

                //if the slot is not empty - swap item
                else if (slot != null && slot.heldItem != null){
                    LastSlotPosition.GetComponent<InventorySlots>().SetHeldItem(slot.heldItem);
                    slot.SetHeldItem(draggedObject);
                    draggedObject = null;
                }

                //if there is not slot and no drop - replace item at last pos
                else if (clickedObject.name != "DropItem"){
                    Debug.Log("Test de cette condition");
                    LastSlotPosition.GetComponent<InventorySlots>().SetHeldItem(draggedObject);
                }

                //drop the item on the map
                else{
                    Vector3 spawnPosition = GameObject.FindWithTag("Player").transform.position;
                    Instantiate(draggedObject.GetComponent<InventoryItem>().displayedItem.prefab, spawnPosition, new Quaternion());

                    LastSlotPosition.GetComponent<InventorySlots>().heldItem = null;
                    Destroy(draggedObject);
                }

                draggedObject = null;
            }
            


        }
    }


}
