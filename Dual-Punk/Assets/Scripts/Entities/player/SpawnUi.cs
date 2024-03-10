using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class SpawnUi : NetworkBehaviour
{
    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject Camera;

    private GameObject LocalUI;
    private GameObject LocalCamera;


    void Start()
    {
        if (IsLocalPlayer)
        {
            LocalUI = Instantiate(UI);
            LocalCamera = Instantiate(Camera);

            LocalUI.name = gameObject.name + " UI";
            LocalCamera.name = gameObject.name + " Camera";

            LocalCamera.GetComponent<CameraController>().Player = gameObject;
            LocalCamera.GetComponent<CameraController>().Camera = LocalCamera.GetComponent<Camera>();
            LocalUI.GetComponent<LocalPlayerReference>().LOCALPLAYER = gameObject;
            LocalUI.GetComponent<LocalPlayerReference>().Camera = LocalCamera;
        }
    }
}
