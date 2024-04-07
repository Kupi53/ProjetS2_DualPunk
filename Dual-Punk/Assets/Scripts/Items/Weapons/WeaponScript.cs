using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Video;


public abstract class WeaponScript : NetworkBehaviour
{
    public PlayerState PlayerState { get; set; }
    public IImpact PlayerRecoil { get; set; }
    
    public bool InHand { get; set; } = false;

    public virtual bool DisplayInfo { get; }
    public virtual float InfoMaxTime { get; }
    public virtual float InfoTimer { get; }

    protected SpriteRenderer _spriteRenderer;
    protected ObjectSpawner _objectSpawner;

    [SerializeField] protected Vector3 _weaponOffset;
    [SerializeField] protected float _weaponDistance;
    [SerializeField] protected float _recoilForce;
    [SerializeField] protected float _impactForce;
    [SerializeField] protected float _cameraShake;

    public Vector3 WeaponOffset { get => _weaponOffset; set => _weaponOffset = value; }

    public abstract void Run(Vector3 position, Vector3 direction);
    public abstract void ResetWeapon();
    


    void Start(){
        _objectSpawner = GameObject.Find("ObjectSpawner").GetComponent<ObjectSpawner>();
        if (IsServer){
            Spawn(gameObject);
        }
    }
}