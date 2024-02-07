using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnUi : NetworkBehaviour
{
    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject Camera;
    private GameObject LocalUI;

    void Start()
    {
        if (IsLocalPlayer)
        {
            Debug.Log("super");
            LocalUI = Instantiate(UI);
            LocalUI.name = gameObject.name + " UI";
            LocalUI.GetComponent<LocalPlayerReference>().LOCALPLAYER = gameObject;
            LocalUI.GetComponent<LocalPlayerReference>().Camera = Camera;
        }
    }
}
