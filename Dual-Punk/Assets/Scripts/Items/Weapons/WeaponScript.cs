using FishNet.Object;
using UnityEngine;
using System;


public abstract class WeaponScript : NetworkBehaviour
{
    [SerializeField] protected Vector3 _weaponOffset;
    [SerializeField] protected float _weaponDistance;
    [SerializeField] protected float _recoilForce;
    [SerializeField] protected float _impactForce;
    [SerializeField] protected float _cameraShake;
    [SerializeField] protected int _pointerSpriteNumber;

    protected SpriteRenderer _spriteRenderer;
    protected ObjectSpawner _objectSpawner;
    protected bool _canAttack;
    protected bool _reloading;

    public PlayerState PlayerState { get; set; }
    public IImpact PlayerRecoil { get; set; }
    public Vector3 WeaponOffset { get => _weaponOffset; set => _weaponOffset = value; }

    public bool InHand { get; set; } = false;
    public virtual bool DisplayInfo { get; }
    public virtual float InfoMaxTime { get; }
    public virtual float InfoTimer { get; }


    protected void Start()
    {
        _canAttack = true;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _objectSpawner = GameObject.Find("ObjectSpawner").GetComponent<ObjectSpawner>();
    }

    protected void Update()
    {
        if (InHand)
        {
            PlayerState.PointerScript.SpriteNumber = _pointerSpriteNumber;
        }
        else
        {
            transform.position += Vector3.up * (float)Math.Cos(Time.time * 5 + _weaponDistance) * 0.001f;
        }
    }


    public abstract void Run(Vector3 position, Vector3 direction);
    public abstract void ResetWeapon();

    public void Drop()
    {
        ResetWeapon();

        InHand = false;
        _canAttack = true;
        transform.position = PlayerState.transform.position + PlayerState.WeaponScript.WeaponOffset;
        transform.rotation = Quaternion.identity;

        if (transform.localScale.y < 0)
        {
            FlipWeapon();
        }

        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }

    public void MovePosition(Vector3 position, Vector3 direction)
    {
        if (InHand && Math.Sign(PlayerState.MousePosition.x - PlayerState.transform.position.x) != Math.Sign(transform.localScale.y))
        {
            FlipWeapon();
        }

        transform.position = position + _weaponOffset + direction * _weaponDistance;
        transform.eulerAngles = new Vector3(0, 0, Methods.GetAngle(direction));
    }

    private void FlipWeapon()
    {
        transform.localScale = new Vector2(transform.localScale.x, -transform.localScale.y);
        _weaponOffset.x = -_weaponOffset.x;
    }


    [ServerRpc]
    private void RemoveAllOwnerShipRPC(NetworkObject networkObject)
    {
        networkObject.RemoveOwnership();
    }


    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Wall"))
        {
            _canAttack = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Wall"))
        {
            _canAttack = true;
        }
    }
}