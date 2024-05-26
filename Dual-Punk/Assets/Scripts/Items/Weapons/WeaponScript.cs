using FishNet.Object;
using UnityEngine;
using System;
using System.Runtime.Serialization;
using FishNet.Connection;


public abstract class WeaponScript : NetworkBehaviour
{
    [SerializeField] private GameObject _rightHand;
    [SerializeField] private GameObject _leftHand;
    [SerializeField] protected Vector3 _weaponOffset;

    [SerializeField] protected int _damage;
    [SerializeField] protected float _range;
    [SerializeField] protected float _recoilForce;
    [SerializeField] protected float _impactForce;
    [SerializeField] protected float _cameraShake;
    [SerializeField] protected float _weaponDistance;
    [SerializeField] protected int _pointerSpriteNumber;

    protected SpriteRenderer _spriteRenderer;
    private SpriteRenderer _rightHandSprite;
    private SpriteRenderer _leftHandSprite;
    protected ObjectSpawner _objectSpawner;
    protected float _currentWeaponDistance;
    protected bool _canAttack;

#nullable enable
    public NetworkConnection? ActualOwner { get; set; }
    public PlayerState? PlayerState { get; set; }
    public EnemyState? EnemyState { get; set; }
    public IImpact? UserRecoil { get; set; }
#nullable disable

    public virtual Vector3 WeaponOffset { get; set; }
    public bool InHand { get; set; } = false;
    public float Range { get => _range; }

    public virtual bool DisplayInfo { get; }
    public virtual float InfoMaxTime { get; }
    public virtual float InfoTimer { get; }


    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rightHandSprite = _rightHand.GetComponent<SpriteRenderer>();
        _leftHandSprite = _leftHand.GetComponent<SpriteRenderer>();
        _objectSpawner = GameObject.FindWithTag("ObjectSpawner").GetComponent<ObjectSpawner>();

        WeaponOffset = _weaponOffset;
        _currentWeaponDistance = _weaponDistance;
        _rightHandSprite.enabled = false;
        _leftHandSprite.enabled = false;
        _canAttack = true;
    }

    protected void Update()
    {
        if (!InHand)
        {
            transform.position += Vector3.up * (float)Math.Cos(Time.time * 5 + _weaponDistance) * 0.001f;
            _rightHandSprite.enabled = false;
            _leftHandSprite.enabled = false;
        }
    }


    public virtual void Run(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        MovePosition(position, direction, targetPoint);

        PlayerState.PointerScript.SpriteNumber = _pointerSpriteNumber;
        if (!_canAttack)
            PlayerState.PointerScript.CanShoot = false;
        else
            PlayerState.PointerScript.CanShoot = true;
    }

    public abstract void EnemyRun(Vector3 position, Vector3 direction, Vector3 targetPoint);
    public abstract void ResetWeapon();


    public void PickUp(GameObject owner)
    {
        InHand = true;
        _canAttack = true;
        _rightHandSprite.enabled = true;
        _leftHandSprite.enabled = true;

        ActualOwner = owner.GetComponent<NetworkObject>().LocalConnection;

        ObjectSpawner.Instance.RemoveParentRpc(gameObject);
        ObjectSpawner.Instance.RemoveOwnershipFromNonOwnersRpc(gameObject, ActualOwner);
    }

    public void Drop()
    {
        ResetWeapon();

        InHand = false;
        ActualOwner = null;
        transform.rotation = Quaternion.identity;
        transform.position = PlayerState == null ? EnemyState.transform.position : PlayerState.transform.position;

        transform.localScale = new Vector2(transform.localScale.x, Math.Abs(transform.localScale.y));
        WeaponOffset = _weaponOffset;

        ObjectSpawner.Instance.ObjectParentToRoomRpc(gameObject);
    }


    protected virtual void MovePosition(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        if (Math.Sign(targetPoint.x - position.x) != Math.Sign(transform.localScale.y))
        {
            // changer seulement s'il n'y a pas d'animation en cours
            transform.localScale = new Vector2(transform.localScale.x, -transform.localScale.y);
            WeaponOffset = new Vector3(-WeaponOffset.x, _weaponOffset.y, 0);
        }

        transform.position = position + WeaponOffset + direction * _currentWeaponDistance;
        transform.eulerAngles = new Vector3(0, 0, Methods.GetAngle(direction));
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