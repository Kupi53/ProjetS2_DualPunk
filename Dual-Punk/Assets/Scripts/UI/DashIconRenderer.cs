using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DashIconRenderer : MonoBehaviour
{
    public RawImage image;

    private float DashEnabledRight;
    private float DashDisabledRight;
    private float transformMultiplier;
    private RectTransform rectTransform;

    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        DashEnabledRight = -rectTransform.offsetMax.x;
        DashDisabledRight = rectTransform.offsetMin.x;
        transformMultiplier = (DashEnabledRight - DashDisabledRight) / PlayerState.DashCooldownMax;
    }

    
    void Update()
    {
        if (PlayerState.DashCooldown > 0)
        {
            image.enabled = true;
            rectTransform.offsetMax = new Vector2(- DashDisabledRight - transformMultiplier * (PlayerState.DashCooldownMax - PlayerState.DashCooldown), rectTransform.offsetMax.y);
        }
        else
        {
            image.enabled = false;
        }
    }
}