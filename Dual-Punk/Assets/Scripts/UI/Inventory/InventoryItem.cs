using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    private DescriptionManager _descriptionManager => GameObject.Find("InventoryManager").GetComponent<DescriptionManager>();
    public InventoryItemData displayedItem;
    public GameObject PrefabInstance;
    public GameObject description => transform.GetChild(0).gameObject;

    

    void Start()
    {
        IconScaling();
        TextSetup();
        description.SetActive(false);
        _iconImage.sprite = displayedItem.icon;
        Canvas.ForceUpdateCanvases();
    }

    private void TextSetup()
    {
        DescriptionPanel descriptionPanel = description.GetComponent<DescriptionPanel>();
        descriptionPanel.SetText(displayedItem.name, displayedItem.description);

        if (PrefabInstance.tag == "Implant" && displayedItem.setItems.Length == 4){
            descriptionPanel.SetImplantText(displayedItem.setName, displayedItem.setItems, displayedItem.setDescription);
            _descriptionManager.UpdateImplantSet();
        }
    }

    private void IconScaling(){
        Sprite spriteSize = displayedItem.icon;
        RectTransform imageSize = _iconImage.rectTransform;

        if(spriteSize.rect.height > imageSize.rect.height && spriteSize.rect.height > spriteSize.rect.width) {
            float ratio = imageSize.rect.height / spriteSize.rect.height;
            int imageWidth = (int)(spriteSize.rect.width * ratio);
            int imageHeight = (int)(spriteSize.rect.height * ratio);
            _iconImage.rectTransform.sizeDelta = new Vector2(imageWidth, imageHeight);
        }
        else if (spriteSize.rect.width > imageSize.rect.width) {
            float ratio = imageSize.rect.width / spriteSize.rect.width;
            int imageWidth = (int)(spriteSize.rect.width * ratio);
            int imageHeight = (int)(spriteSize.rect.height * ratio);
            _iconImage.rectTransform.sizeDelta = new Vector2(imageWidth, imageHeight);
        }
    }
}