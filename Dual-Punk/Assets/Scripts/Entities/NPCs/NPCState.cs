using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPCState : NetworkBehaviour
{
    [SerializeField] private int _health;
    [SerializeField] protected float _walkSpeed;
    [SerializeField] protected float _runSpeed;

    public virtual Vector3 TargetPoint { get; set; }
    public Vector3 MoveDirection { get; set; }
    public float MoveSpeed { get; set; }
    public bool Move { get; set; }


    protected void Start()
    {
        MoveSpeed = _walkSpeed;
        Move = true;
    }
}