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
    [SerializeField] private RawImage Image;

    private float DashEnabledRight;
    private float DashDisabledRight;
    private float transformMultiplier;
    private RectTransform rectTransform;
    private PlayerState playerState;


    void Start()
    {
        rectTransform = Image.GetComponent<RectTransform>();
        DashEnabledRight = -rectTransform.offsetMax.x;
        DashDisabledRight = rectTransform.offsetMin.x;
        playerState = gameObject.transform.root.gameObject.GetComponent<LocalPlayerReference>().playerState;
        transformMultiplier = (DashEnabledRight - DashDisabledRight) / playerState.DashCooldownMax;
    }
    
    void Update()
    {
        if (playerState.DashCooldown > 0)
        {
            rectTransform.offsetMax = new Vector2(-DashDisabledRight - transformMultiplier * (playerState.DashCooldownMax - playerState.DashCooldown), rectTransform.offsetMax.y);
        }
    }
}