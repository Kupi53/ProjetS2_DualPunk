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

    private float height;
    private float width;

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
        Debug.Log(width + " " + height);
    }


}
