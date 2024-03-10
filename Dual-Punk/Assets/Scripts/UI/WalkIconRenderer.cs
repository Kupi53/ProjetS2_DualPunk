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
        playerState = transform.root.GetComponent<LocalPlayerReference>().PlayerState;
        Image.enabled = false;
    }

    void Update()
    {
        
    }
}
