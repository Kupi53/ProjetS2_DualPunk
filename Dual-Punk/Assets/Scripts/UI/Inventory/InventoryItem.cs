using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] Image IconImage;

    public InventoryItemData displayedItem;

    public GameObject description => transform.GetChild(0).gameObject;
    private GameObject stackIcon => transform.GetChild(3).gameObject;
    

    void Start()
    {
        TextSetup();
        ActiveStack();
        description.SetActive(false);
        IconImage.sprite = displayedItem.icon;
        Canvas.ForceUpdateCanvases();
    }

    private void TextSetup()
    {
        DescriptionPanel descriptionPanel = description.GetComponent<DescriptionPanel>();
        descriptionPanel.SetText(displayedItem.name, displayedItem.description);
    }

    private void ActiveStack()
    {
        if (displayedItem.prefab.tag == "Item")
            stackIcon.SetActive(true);
    }
}