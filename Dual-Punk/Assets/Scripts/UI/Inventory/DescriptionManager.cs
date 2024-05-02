using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DescriptionManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private InventorySlots[] _implantSlots = new InventorySlots[4];
    private GameObject _descriptionWindow;
    private Coroutine _storedCouroutine;
    private bool _inCD;

    public Coroutine GetStoredCoroutine()
    {
        return _storedCouroutine;
    }

    public bool GetInCD()
    {
        return _inCD;
    }

    public GameObject GetDescriptionWindow()
    {
        return _descriptionWindow;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject.tag == "InventorySlot")
        {
            GameObject currentItem = eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlots>().heldItem;

            if(currentItem != null) {
                _descriptionWindow = currentItem.transform.GetChild(0).gameObject;
                _storedCouroutine = StartCoroutine(DisplayCooldown());
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_inCD) {
            StopTheCoroutine();
        }
        if (_descriptionWindow != null) {
            _descriptionWindow.SetActive(false);
        }
    }

    private IEnumerator DisplayCooldown()
    {
    _inCD = true;
    yield return new WaitForSeconds(1);
    _inCD = false;
    _descriptionWindow.SetActive(true);
    }

    public void StopTheCoroutine()
    {
        StopCoroutine(_storedCouroutine);
    }

    public void UpdateImplantSet(){

        Debug.LogWarning("1");

        for (int i = 0 ; i < _implantSlots.Length; i++)
        {
            if (_implantSlots[i].heldItem != null)
            {
                for (int j = 0 ; j < 4; j++)
                {
                    Text updateText = _implantSlots[i].heldItem.transform.GetChild(0).transform.GetChild(3+j).GetComponent<Text>();
                    Color newColor = updateText.color;

                    if (_implantSlots[j].heldItem != null){

                        string itemInSlotName = _implantSlots[j].heldItem.GetComponent<InventoryItem>().displayedItem.name;

                        if(updateText.text.Contains(itemInSlotName)){
                            newColor.a = 1f;
                        }
                        else{
                            newColor.a = 0.3f;
                        }
                    }
                    else{
                        newColor.a = 0.3f;
                    }
                    
                    updateText.color = newColor;
                }
            }
        }
    }
}