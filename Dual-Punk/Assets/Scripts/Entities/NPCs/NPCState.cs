using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPCState : NetworkBehaviour
{
    [SerializeField] private LayerMask _layerMask;

    public LayerMask LayerMask { get => _layerMask; }
    public Vector3 TargetPoint { get; set; }
    public bool Stop { get; set; }
    public bool Move { get; set; }
    public bool Run { get; set; }

    #nullable enable
    public Room? ParentRoom { get; set; }
    #nullable disable


    protected void Awake()
    {
        TargetPoint = Vector3.zero;
        Stop = false;
        Move = true;
        Run = false;
    }
}