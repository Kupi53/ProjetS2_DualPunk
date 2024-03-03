using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class WalkIconRenderer : MonoBehaviour
{
    [SerializeField] private RawImage Image;
    private PlayerState playerState;

    void Start()
    {
        playerState = transform.root.gameObject.GetComponent<LocalPlayerReference>().PlayerState;
    }

    void Update()
    {
        if (playerState.Walking)
        {
            Image.enabled = true;
        }
        else
        {
            Image.enabled = false;
        }
    }
}
