using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class MaxHealth : MonoBehaviour
{
    public TextMeshProUGUI text;
    private PlayerState? playerState;


    void Update()
    {
        if (playerState == null)
        {
            playerState = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId].PlayerObject.gameObject.GetComponent<PlayerState>();
        }
        else
        {
            text.text = playerState.MaxHealth.ToString();
        }
    }
}
