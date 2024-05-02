using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionPanel : MonoBehaviour
{
    [SerializeField]
    private Text Name;
    [SerializeField]
    private Text description;
    [SerializeField]
    private LayoutElement formatage;
    [SerializeField]
    private int maxCharacter;
    [SerializeField] Text SetName;
    [SerializeField] Text[] Items;

    public float height;
    public float width;

    public void SetText(string namestr, string descriptionstr)
    {
        Name.text = namestr + "\n";
        description.text = descriptionstr;

        int nbNameChar = Name.text.Length;
        int nbDescriptionChar = description.text.Length;

        formatage.enabled = nbNameChar > maxCharacter || nbDescriptionChar > maxCharacter;

        Canvas.ForceUpdateCanvases();
        height = GetComponent<RectTransform>().rect.height;
        width = GetComponent<RectTransform>().rect.width;

        RectTransform inventoryItemRect = transform.parent.GetComponent<RectTransform>();

        transform.localPosition = new Vector3(inventoryItemRect.rect.width/2-15 + width/2, inventoryItemRect.rect.height/2-15 + height/2, 0);
    }

    public void SetImplantText(string setName, string[] items){
        for(int i = 2; i < 7; i++){
            transform.GetChild(i).gameObject.SetActive(true);
        }
        SetName.text = "\nSet " + setName + ":";
        for(int j = 0  ; j < items.Length; j ++){
            Items[j].text = "    " + $"{j+1}: {items[j]} ({ImplantTypeString(j)})";
        }
    }

    private string ImplantTypeString(int i){
        string res = "";

        switch(i){
            case 0:
                res = "neuralink";
                break;
            case 1:
                res = "body";
                break;
            case 2:
                res = "arms";
                break;
            default:
                res =  "legs";
                break;
        }

        return res;
    }

}
