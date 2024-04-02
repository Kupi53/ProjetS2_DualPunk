using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{

    private GameObject StackIcon => transform.GetChild(3).gameObject;
    public InventoryItemData displayedItem;
    public GameObject description => transform.GetChild(0).gameObject;
    [SerializeField] Image IconImage;

    void Start(){
        TextSetup();
        //TextPositionning();
        ActiveStack();
    }
    void Update()
    {
        IconImage.sprite = displayedItem.icon;
    }

    private void TextSetup(){
        DescriptionPanel descriptionPanel = description.GetComponent<DescriptionPanel>();
        descriptionPanel.SetText(displayedItem.name, displayedItem.description);
    }

    private void ActiveStack(){
        if(displayedItem.prefab.tag == "Item"){
            StackIcon.SetActive(true);
        }
    }

    /*void TextPositionning(){
        RectTransform descriptionSize = description.GetComponent<RectTransform>();
        description.transform.position = new Vector3(description.transform.position.x/2 + descriptionSize.rect.width/2
                                                    ,description.transform.position.y/2 + descriptionSize.rect.height/2,0);
    }*/




}
