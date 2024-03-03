using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public abstract class WeaponScript : NetworkBehaviour
{
    public PlayerState PlayerState { get; set; }
    public PointerScript PointerScript { get; set; }
    public bool InHand { get; set; } = false;

    public Vector3 WeaponOffset { get; set; }
    public float WeaponDistance { get; set; }

    protected SpriteRenderer _spriteRenderer;

    public abstract void Run(Vector3 position, Vector3 rotation);

    public abstract void Reset();
}