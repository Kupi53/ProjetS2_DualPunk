using FishNet.Object;
using UnityEngine;
using System;


public abstract class WeaponScript : NetworkBehaviour
{
    [SerializeField] private GameObject _rightHand;
    [SerializeField] private GameObject _leftHand;
    [SerializeField] protected Vector3 _weaponOffset;

    [SerializeField] protected float _weaponDistance;
    [SerializeField] protected float _recoilForce;
    [SerializeField] protected float _impactForce;
    [SerializeField] protected float _cameraShake;
    [SerializeField] protected int _pointerSpriteNumber;

    protected SpriteRenderer _spriteRenderer;
    private SpriteRenderer _rightHandSprite;
    private SpriteRenderer _leftHandSprite;
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
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rightHandSprite = _rightHand.GetComponent<SpriteRenderer>();
        _leftHandSprite = _leftHand.GetComponent<SpriteRenderer>();
        _objectSpawner = GameObject.Find("ObjectSpawner").GetComponent<ObjectSpawner>();

        _canAttack = true;
        _reloading = false;
        _rightHandSprite.enabled = false;
        _leftHandSprite.enabled = false;
    }

    protected void Update()
    {
        if (InHand) {
            PlayerState.PointerScript.SpriteNumber = _pointerSpriteNumber;
        } else {
            transform.position += Vector3.up * (float)Math.Cos(Time.time * 5 + _weaponDistance) * 0.001f;
            _rightHandSprite.enabled = false;
            _leftHandSprite.enabled = false;
        }
    }


    public abstract void Run(Vector3 position, Vector3 direction);
    public abstract void ResetWeapon();


    public void PickUp(PlayerState playerState, IImpact playerRecoil)
    {
        InHand = true;
        _canAttack = true;
        _rightHandSprite.enabled = true;
        _leftHandSprite.enabled = true;
        PlayerState = playerState;
        PlayerRecoil = playerRecoil;
    }

    public void Drop()
    {
        ResetWeapon();

        InHand = false;
        transform.position = PlayerState.transform.position + PlayerState.WeaponScript.WeaponOffset;
        transform.rotation = Quaternion.identity;

        if (transform.localScale.y < 0)
        {
            transform.localScale = new Vector2(transform.localScale.x, Math.Abs(transform.localScale.y));
            _weaponOffset.x = Math.Abs(_weaponOffset.x);
        }

        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }


    public void MovePosition(Vector3 position, Vector3 direction)
    {
        if (InHand && Math.Sign(PlayerState.MousePosition.x - PlayerState.transform.position.x) != Math.Sign(transform.localScale.y))
        {
            transform.localScale = new Vector2(transform.localScale.x, -transform.localScale.y);
            _weaponOffset.x = -_weaponOffset.x;
        }

        transform.position = position + _weaponOffset + direction * _weaponDistance;
        transform.eulerAngles = new Vector3(0, 0, Methods.GetAngle(direction));
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