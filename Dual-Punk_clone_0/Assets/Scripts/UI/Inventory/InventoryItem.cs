using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{

    public GameObject displayedItem;
    [SerializeField] Image IconImage;

    void Update()
    {
        IconImage.sprite = displayedItem.GetComponent<SpriteRenderer>().sprite;
    }
}
