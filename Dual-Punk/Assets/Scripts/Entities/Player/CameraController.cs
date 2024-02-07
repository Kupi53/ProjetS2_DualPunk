using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Xml.Schema;
using Unity.Netcode;
using UnityEngine;
using Unity.Networking;


public class CameraController : NetworkBehaviour
{
    [SerializeField] private GameObject Player;

    private Vector3 Offset;
    private Vector3 Velocity;
    private float SmoothTime;


    void Start()
    {
        Offset = new Vector3(0, 0, -10);
        Velocity = Vector3.zero;
        SmoothTime = 0.25f;
        Debug.Log("c");
    }


    void Update()
    {
        Vector3 playerPosition = Player.transform.position + Offset;
        transform.position = Vector3.SmoothDamp(transform.position, playerPosition, ref Velocity, SmoothTime);
    }
}
