using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{

    public InventoryItemData displayedItem;
    public GameObject description => transform.GetChild(0).gameObject;
    [SerializeField] Image IconImage;

    void Start(){
        TextSetup();
        //TextPositionning();
    }
    void Update()
    {
        IconImage.sprite = displayedItem.icon;
    }

    void TextSetup(){
        DescriptionPanel descriptionPanel = description.GetComponent<DescriptionPanel>();
        descriptionPanel.SetText(displayedItem.name, displayedItem.description);
    }

    /*void TextPositionning(){
        RectTransform descriptionSize = description.GetComponent<RectTransform>();
        description.transform.position = new Vector3(description.transform.position.x/2 + descriptionSize.rect.width/2
                                                    ,description.transform.position.y/2 + descriptionSize.rect.height/2,0);
    }*/
}
