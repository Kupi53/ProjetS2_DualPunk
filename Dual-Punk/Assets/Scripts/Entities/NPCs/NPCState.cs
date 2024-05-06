using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPCState : NetworkBehaviour
{
    public GameObject Target { get; set; }
    public Vector3 Direction { get; set; }
    public bool Move { get; set; }
    public bool Run { get; set; }


    protected void Start()
    {
        Move = true;
        Run = false;
    }
}