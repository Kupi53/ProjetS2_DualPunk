using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    private DescriptionManager _descriptionManager => GameObject.Find("InventoryManager").GetComponent<DescriptionManager>();
    public InventoryItemData displayedItem;
    public GameObject description => transform.GetChild(0).gameObject;

    

    void Start()
    {
        TextSetup();
        description.SetActive(false);
        _iconImage.sprite = displayedItem.icon;
        
        Canvas.ForceUpdateCanvases();
    }

    private void TextSetup()
    {
        DescriptionPanel descriptionPanel = description.GetComponent<DescriptionPanel>();
        descriptionPanel.SetText(displayedItem.name, displayedItem.description);

        if (displayedItem.prefab.tag == "Implant"){
            descriptionPanel.SetImplantText(displayedItem.setName, displayedItem.setItems);
            _descriptionManager.UpdateImplantSet();
        }
    }
}