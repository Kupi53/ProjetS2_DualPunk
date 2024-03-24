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

        // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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

            if (slot != null && slot.HeldItem != null){
                // Debug.Log("tu devrais avoir l'item dans la main la.");
                draggedObject = slot.HeldItem;
                slot.HeldItem = null;
                LastSlotPosition = clickedObject;
                // Debug.Log(LastSlotPosition);
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

                if(slot != null && slot.HeldItem == null){

                    slot.SetHeldItem(draggedObject);
                    draggedObject = null;

                }

                else if (slot != null && slot.HeldItem != null){
                    LastSlotPosition.GetComponent<InventorySlots>().SetHeldItem(slot.HeldItem);
                    slot.SetHeldItem(draggedObject);
                    draggedObject = null;
                }
            }
            

            // Remets l'item a sa place dans l'inventaire s'il est pas dans un des slots.
            else {
                LastSlotPosition.GetComponent<InventorySlots>().SetHeldItem(draggedObject);
                draggedObject = null;
            }

        }
    }


}
