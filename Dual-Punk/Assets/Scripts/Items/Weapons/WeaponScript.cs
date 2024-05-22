using FishNet.Object;
using UnityEngine;
using System;
using System.Runtime.Serialization;


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
    protected bool _canAttack;
    protected bool _reloading;

#nullable enable
    public PlayerState? PlayerState { get; set; }
    public EnemyState? EnemyState { get; set; }
    public IImpact? UserRecoil { get; set; }
#nullable disable
    public Vector3 WeaponOffset { get => _weaponOffset;}
    public bool InHand { get; set; } = false;
    public float Range { get => _range; }

    public virtual bool DisplayInfo { get; }
    public virtual float InfoMaxTime { get; }
    public virtual float InfoTimer { get; }


    protected void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rightHandSprite = _rightHand.GetComponent<SpriteRenderer>();
        _leftHandSprite = _leftHand.GetComponent<SpriteRenderer>();
        _objectSpawner = GameObject.FindWithTag("ObjectSpawner").GetComponent<ObjectSpawner>();

        _canAttack = true;
        _reloading = false;
        _rightHandSprite.enabled = false;
        _leftHandSprite.enabled = false;
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

        ObjectSpawner.Instance.ObjectParentToGameObjectRpc(gameObject, owner);
    }

    public void Drop()
    {
        ResetWeapon();

        InHand = false;
        transform.rotation = Quaternion.identity;
        transform.position = PlayerState == null ? EnemyState.transform.position : PlayerState.transform.position;

        if (transform.localScale.y < 0)
        {
            transform.localScale = new Vector2(transform.localScale.x, Math.Abs(transform.localScale.y));
            _weaponOffset.x = Math.Abs(_weaponOffset.x);
        }

        ObjectSpawner.Instance.ObjectParentToRoomRpc(gameObject);
    }


    public void MovePosition(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        if (InHand && Math.Sign(targetPoint.x - position.x) != Math.Sign(transform.localScale.y))
        {
            transform.localScale = new Vector2(transform.localScale.x, -transform.localScale.y);
            _weaponOffset.x = -_weaponOffset.x;
        }

        transform.position = position + _weaponOffset + direction * _weaponDistance;
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