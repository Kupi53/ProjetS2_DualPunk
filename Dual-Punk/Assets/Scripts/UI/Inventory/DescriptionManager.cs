using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DescriptionManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject descriptionWindow;
    private Coroutine storedCouroutine;
    private bool inCD;

    public Coroutine GetStoredCoroutine(){
        return storedCouroutine;
     }
    public bool GetInCD(){
        return inCD;
    }
    public GameObject GetDescriptionWindow(){
        return descriptionWindow;
    }

    public void OnPointerEnter(PointerEventData eventData){
        if (eventData.pointerCurrentRaycast.gameObject.tag == "InventorySlot"){
            GameObject currentItem = eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlots>().heldItem;

            if(currentItem != null){
                descriptionWindow = currentItem.transform.GetChild(0).gameObject;
                storedCouroutine = StartCoroutine(DisplayCooldown());
            }
        }
    }

     public void OnPointerExit(PointerEventData eventData){
        if (inCD) {
            StopTheCoroutine();
        }
        if(descriptionWindow != null){
            descriptionWindow.SetActive(false);
        }
     }

     private IEnumerator DisplayCooldown(){
        inCD = true;
        yield return new WaitForSeconds(1);
        inCD = false;
        descriptionWindow.SetActive(true);
     }

    public void StopTheCoroutine(){
        StopCoroutine(storedCouroutine);
    }

}
