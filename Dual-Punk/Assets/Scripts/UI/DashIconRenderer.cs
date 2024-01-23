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
    private PlayerState? playerState;


    void Start()
    {
        rectTransform = image.GetComponent<RectTransform>();
        DashEnabledRight = -rectTransform.offsetMax.x;
        DashDisabledRight = rectTransform.offsetMin.x;
    }

    
    void Update()
    {
        if (playerState == null)
        {
            playerState = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId].PlayerObject.gameObject.GetComponent<PlayerState>();
            try
            {
                transformMultiplier = (DashEnabledRight - DashDisabledRight) / playerState.DashCooldownMax;
            }
            catch (Exception) { }
        }
        else
        {
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
}