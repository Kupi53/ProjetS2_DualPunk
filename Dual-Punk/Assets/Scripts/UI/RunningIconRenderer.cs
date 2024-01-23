using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class RunningIconRenderer : MonoBehaviour
{
    public RawImage image;
    private PlayerState playerState;

    void Start(){
        playerState = gameObject.transform.root.gameObject.GetComponent<LocalPlayerReference>().LOCALPLAYER.gameObject.GetComponent<PlayerState>();
    }
    void Update()
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
