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
    [SerializeField] Image IconImage;

    void Update()
    {
        IconImage.sprite = displayedItem.icon;
    }
}
