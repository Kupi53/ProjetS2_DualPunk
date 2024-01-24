using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class CurrentHealth : MonoBehaviour
{
    public TextMeshProUGUI text;
    private PlayerState playerState;

    void Start()
    {
        playerState = gameObject.transform.root.gameObject.GetComponent<LocalPlayerReference>().playerState;
    }

    void Update()
    {
        text.text = playerState.Health.ToString();
    }
}
