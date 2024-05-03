using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class selectedSlotManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image _inventoryPanel;
    private GameObject _selectedSlotIcon;
    private bool _activeness;



    void Update()
    {
        if (_selectedSlotIcon != null)
        {
            if (_activeness)
            {
                _selectedSlotIcon.SetActive(true);
            }
            else
            {
                _selectedSlotIcon.SetActive(false);
                _selectedSlotIcon = null;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData){
        if(eventData.pointerCurrentRaycast.gameObject.tag == "InventorySlot"){

            _selectedSlotIcon = eventData.pointerCurrentRaycast.gameObject.transform.GetChild(0).gameObject;
            _activeness = true;
        }

        //Debug.Log("enter" + eventData.pointerCurrentRaycast.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData){
        if(_selectedSlotIcon != null){
            _activeness = false;
        }

        //Debug.Log("exit" + eventData.pointerCurrentRaycast.gameObject);
    }

    public bool GetActiveness(){
        return _activeness;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
        InventorySlots slot = clickedObject.GetComponent<InventorySlots>();

        if(slot != null){
            _activeness = false;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;

        if (clickedObject != null){

            InventorySlots slot = clickedObject.GetComponent<InventorySlots>();

            if(slot != null){
                _selectedSlotIcon = eventData.pointerCurrentRaycast.gameObject.transform.GetChild(0).gameObject;
                _activeness = true;
            }
        }

    }

}
