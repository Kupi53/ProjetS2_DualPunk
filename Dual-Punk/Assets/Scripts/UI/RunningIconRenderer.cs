using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class RunningIconRenderer : MonoBehaviour
{
    public RawImage image;
    private PlayerState? playerState;

    void Update()
    {
        if (playerState == null)
        {
            playerState = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId].PlayerObject.gameObject.GetComponent<PlayerState>();
        }
        else
        {
            if (playerState.Walking)
            {
                image.enabled = false;
            }
            else
            {
                image.enabled = true;
            }
        }
    }
}
