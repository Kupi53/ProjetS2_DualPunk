using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedSlotManager : MonoBehaviour
{
    [SerializeField] private Image _inventoryPanel;
    [SerializeField] private Image _dropPanel;
    private GameObject _selectedSlot;
    private GameObject _selectedSlotIcon;
    private bool _aboveSlot;




    void Update()
    {
        UpdateSelectedSlotIcon();
        
    }

    public void UpdateSelectedSlotIcon(){
		
        bool found = false;
		PointerEventData pointerData = new PointerEventData(EventSystem.current);
		pointerData.position = Input.mousePosition;

		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerData, results);

        
        for(int i = 0; i < results.Count; i++){
            if(results[i].gameObject.tag == "InventorySlot") 
            {
                found = true;
                
                if (!_aboveSlot)
                {
                    if (results[i].gameObject != _selectedSlot){
                        _aboveSlot = true;
                        _selectedSlot = results[i].gameObject;
                        _selectedSlotIcon = _selectedSlot.transform.GetChild(0).gameObject;
                        _selectedSlotIcon.SetActive(true);
                    }
                }

            }
        } 

        if(!found && _selectedSlot != null){
            _selectedSlotIcon.SetActive(false);
            _selectedSlot = null;
            _aboveSlot = false;
        }
	}
}
