using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPCState : NetworkBehaviour
{
    public GameObject Target { get; set; }
    public bool Move { get; set; }
    public bool Run { get; set; }

    public Vector3 TargetPoint { get => Target.transform.position; }


    protected void Awake()
    {
        Move = true;
        Run = false;
    }
}