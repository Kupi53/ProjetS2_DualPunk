using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{

    private GameObject stackIcon => transform.GetChild(3).gameObject;
    public InventoryItemData displayedItem;
    public GameObject description => transform.GetChild(0).gameObject;
    [SerializeField] Image IconImage;

    void Start(){
        TextSetup();
        ActiveStack();
        description.SetActive(false);
        IconImage.sprite = displayedItem.icon;
    }

    private void TextSetup(){
        DescriptionPanel descriptionPanel = description.GetComponent<DescriptionPanel>();
        descriptionPanel.SetText(displayedItem.name, displayedItem.description);
    }

    private void ActiveStack(){
        if(displayedItem.prefab.tag == "Item"){
            stackIcon.SetActive(true);
        }
    }

}
