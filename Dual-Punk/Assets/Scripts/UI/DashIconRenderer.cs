using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;


public class DashIconRenderer : MonoBehaviour
{
    public RawImage image;

    private float DashEnabledRight;
    private float DashDisabledRight;
    private float transformMultiplier;
    private RectTransform rectTransform;
    private PlayerState playerState;


    void Start()
    {
        rectTransform = image.GetComponent<RectTransform>();
        DashEnabledRight = -rectTransform.offsetMax.x;
        DashDisabledRight = rectTransform.offsetMin.x;
        playerState = gameObject.transform.root.gameObject.GetComponent<LocalPlayerReference>().LOCALPLAYER.gameObject.GetComponent<PlayerState>();
    }

    
    void Update()
    {
        transformMultiplier = (DashEnabledRight - DashDisabledRight) / playerState.DashCooldownMax;
        if (playerState.DashCooldown > 0)
        {
            image.enabled = true;
            rectTransform.offsetMax = new Vector2(-DashDisabledRight - transformMultiplier * (playerState.DashCooldownMax - playerState.DashCooldown), rectTransform.offsetMax.y);
        }
        else
        {
            image.enabled = false;
        }
    }
}