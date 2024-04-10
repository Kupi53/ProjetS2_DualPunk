using FishNet.Object;
using UnityEngine;
using System;


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

    public void Drop()
    {
        ResetWeapon();
        InHand = false;
        transform.position = PlayerState.transform.position + PlayerState.WeaponScript.WeaponOffset;
        transform.rotation = Quaternion.identity;

        if (_spriteRenderer.flipY)
        {
            FlipWeaponServerRpc(false);
        }
    }

    protected void MovePosition(Vector3 position, Vector3 direction)
    {
        FlipWeaponServerRpc(PlayerState.MousePosition.x < PlayerState.transform.position.x);
        
        transform.position = position + _weaponOffset + direction * _weaponDistance;
        transform.eulerAngles = new Vector3(0, 0, Methods.GetAngle(direction));
    }

    
    [ServerRpc (RequireOwnership = false)]
    protected void FlipWeaponServerRpc(bool flip)
    {
        FlipWeaponClientRpc(flip);
    }


    [ObserversRpc]
    protected void FlipWeaponClientRpc(bool flip)
    {
        if (_spriteRenderer.flipY == !flip)
        {
            _spriteRenderer.flipY = flip;
            _weaponOffset.x = -_weaponOffset.x;
        }
    }


    void Awake()
    {
        _objectSpawner = GameObject.Find("ObjectSpawner").GetComponent<ObjectSpawner>();
    }
}