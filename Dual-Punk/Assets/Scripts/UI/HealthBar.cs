using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.Netcode;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using System;


public class HealthBar : MonoBehaviour
{
    public Image image;

    private float HealthMaxRight;
    private float HealthMinRight;
    private float transformMultiplier;
    private RectTransform rectTransform;
    private PlayerState? playerState;


    // Start is called before the first frame update
    void Start()
    {
        rectTransform = image.GetComponent<RectTransform>();
        HealthMaxRight = rectTransform.offsetMin.x;
        HealthMinRight = -rectTransform.offsetMax.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState == null)
        {
            playerState = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId].PlayerObject.gameObject.GetComponent<PlayerState>();
            try
            {
                transformMultiplier = (HealthMinRight - HealthMaxRight) / playerState.MaxHealth;
            }
            catch (Exception) { }
        }
        else
        {
            rectTransform.offsetMax = new Vector2(-HealthMaxRight - transformMultiplier * (playerState.MaxHealth - playerState.Health), rectTransform.offsetMax.y);
        }
    }


    [ContextMenu("DecreaseHealth")]
    public void DecreaseHealth()
    {
        Debug.Log("HealthDecreased");
        playerState.Health -= 9;
        if (playerState.Health < 0)
            playerState.Health = 0;
    }

    [ContextMenu("IncreaseHealth")]
    public void IncreaseHealth()
    {
        Debug.Log("HealthIncreased");
        playerState.Health += 9;
        if (playerState.Health > playerState.MaxHealth)
            playerState.Health = playerState.MaxHealth;
    }
}
