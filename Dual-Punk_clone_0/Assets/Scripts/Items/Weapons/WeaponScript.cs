using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
//using Unity.Netcode;
using UnityEngine;


public abstract class WeaponScript : MonoBehaviour
{
    public PlayerState PlayerState { get; set; }

    public bool InHand { get; set; } = false;

    [SerializeField] protected float _weaponDistance;
    [SerializeField] protected Vector3 _weaponOffset;

    public Vector3 WeaponOffset { get => _weaponOffset; set => _weaponOffset = value; }

    protected SpriteRenderer _spriteRenderer;

    public abstract void Run(Vector3 position, Vector3 rotation);

    public abstract void Reset();
}