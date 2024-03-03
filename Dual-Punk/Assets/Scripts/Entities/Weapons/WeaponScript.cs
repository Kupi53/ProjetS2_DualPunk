using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public abstract class WeaponScript : NetworkBehaviour
{
    public bool InHand { get; set; }
    public Vector3 WeaponOffset { get; set; }
    public PointerScript PointerScript { get; set; }
    public PlayerState PlayerState { get; set; }

    public abstract void Run(Vector3 position, Vector3 rotation);

    public abstract void Reset();
}