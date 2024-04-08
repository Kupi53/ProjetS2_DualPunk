using System.Collections;
using System.Collections.Generic;
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

    public float height;
    public float width;

    public void SetText(string namestr, string descriptionstr){
        Name.text = namestr + "\n";
        description.text = descriptionstr;

        int nbNameChar = Name.text.Length;
        int nbDescriptionChar = description.text.Length;

        if(nbNameChar > maxCharacter || nbDescriptionChar > maxCharacter) {
            formatage.enabled = true;

        }
        else formatage.enabled = false;

        Canvas.ForceUpdateCanvases();
        height = GetComponent<RectTransform>().rect.height;
        width = GetComponent<RectTransform>().rect.width;

        RectTransform inventoryItemRect = transform.parent.GetComponent<RectTransform>();
        Debug.Log(inventoryItemRect.rect.width + " " + inventoryItemRect.rect.height/2 + height);

        transform.localPosition = new Vector3(inventoryItemRect.rect.width/2-15 + width/2, inventoryItemRect.rect.height/2-15 + height/2, 0);
    }


}
