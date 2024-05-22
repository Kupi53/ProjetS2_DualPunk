using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPCState : NetworkBehaviour
{
    public bool Stop { get; set; }
    public bool Run { get; set; }
    public virtual Vector3 TargetPoint { get; set; }


    protected void Awake()
    {
        Stop = false;
        Run = false;
    }
}