using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class MaxHealth : MonoBehaviour
{
    public TextMeshProUGUI text;
    private PlayerState playerState;

    void Start(){
        playerState = gameObject.transform.root.gameObject.GetComponent<LocalPlayerReference>().LOCALPLAYER.gameObject.GetComponent<PlayerState>();
    }
    void Update()
    {
        text.text = playerState.MaxHealth.ToString();
    }
}
