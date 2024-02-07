using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnUi : NetworkBehaviour
{
    [SerializeField] private GameObject UI;
    private GameObject LocalUI;

    void Start()
    {
        if (IsLocalPlayer)
        {
            GameObject LocalUI = Instantiate(UI);
            LocalUI.name = gameObject.name + " UI";
            LocalUI.GetComponent<LocalPlayerReference>().LOCALPLAYER = gameObject;
        }
    }
}
