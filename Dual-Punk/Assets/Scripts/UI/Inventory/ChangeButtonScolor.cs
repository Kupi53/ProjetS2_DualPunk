using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeButtonColor : MonoBehaviour
{
    [SerializeField] private Color whiteColor;
    [SerializeField] private Color normalColor;
    [SerializeField] private Button button;

    public void ChangeColor(){
        ColorBlock nwColor = button.colors;
        if(button.colors.selectedColor == whiteColor){
            nwColor.selectedColor = normalColor;
        }
        else {
            nwColor.selectedColor = whiteColor;
        }
        button.colors = nwColor;
    } 
}
