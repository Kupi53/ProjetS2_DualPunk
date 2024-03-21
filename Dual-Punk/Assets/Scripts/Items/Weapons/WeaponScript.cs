using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Video;


public abstract class WeaponScript : NetworkBehaviour
{
    public PlayerState PlayerState { get; set; }

    public bool InHand { get; set; } = false;

    public virtual bool DisplayInfo { get; }
    public virtual float InfoMaxTime { get; }
    public virtual float InfoTimer { get; }

    protected SpriteRenderer _spriteRenderer;

    [SerializeField] protected float _weaponDistance;
    [SerializeField] protected Vector3 _weaponOffset;

    public Vector3 WeaponOffset { get => _weaponOffset; set => _weaponOffset = value; }

    public abstract void Run(Vector3 position, Vector3 direction);
    public abstract void Reset();
}