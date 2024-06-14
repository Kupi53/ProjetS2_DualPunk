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

    private SpriteRenderer _rightHandSprite;
    private SpriteRenderer _leftHandSprite;
    protected ObjectSpawner _objectSpawner;
    protected string _ownerType;

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


    protected void Awake()
    {
        _rightHandSprite = _rightHand.GetComponent<SpriteRenderer>();
        _leftHandSprite = _leftHand.GetComponent<SpriteRenderer>();
        _objectSpawner = GameObject.FindWithTag("ObjectSpawner").GetComponent<ObjectSpawner>();

        WeaponOffset = _weaponOffset;
        _rightHandSprite.enabled = false;
        _leftHandSprite.enabled = false;
    }

    protected void Update()
    {
        if (PlayerState != null)
            _ownerType = "Player";
        else
            _ownerType = "Enemy";

        if (!InHand)
        {
            transform.position += Vector3.up * (float)Math.Cos(Time.time * 5 + _damage) * 0.001f;
            _rightHandSprite.enabled = false;
            _leftHandSprite.enabled = false;
        }
        else
        {
            _rightHandSprite.enabled = true;
            _leftHandSprite.enabled = true;
        }
    }


    public abstract void Run(Vector3 position, Vector3 direction, Vector3 targetPoint);
    public abstract void EnemyRun(Vector3 position, Vector3 direction, Vector3 targetPoint);
    public abstract void MovePosition(Vector3 position, Vector3 direction, Vector3 targetPoint);
    public abstract void ResetWeapon();


    public void PickUp(GameObject owner)
    {
        Debug.Log("AU SEOUR " + gameObject);
        UpdateInHandSer(true);

        PlayerState = owner.GetComponent<PlayerState>();
        EnemyState = owner.GetComponent<EnemyState>();
        UserRecoil = owner.GetComponent<IImpact>();
        ActualOwner = owner.GetComponent<NetworkObject>().LocalConnection;

        ObjectSpawner.Instance.RemoveParentRpc(gameObject);
        ObjectSpawner.Instance.RemoveOwnershipFromNonOwnersRpc(gameObject, ActualOwner);
    }

    public void Drop()
    {
        ResetWeapon();
        UpdateInHandSer(false);

        WeaponOffset = _weaponOffset;
        transform.rotation = Quaternion.identity;
        transform.position = PlayerState == null ? EnemyState.transform.position : PlayerState.transform.position;
        transform.localScale = new Vector2(transform.localScale.x, Math.Abs(transform.localScale.y));

        if (PlayerState != null )
            PlayerState.Firing = false;

        ActualOwner = null;
        PlayerState = null;
        EnemyState = null;

        if (!GameManager.Instance.InTutorial)
        {
            ObjectSpawner.Instance.ObjectParentToRoomRpc(gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void UpdateInHandSer(bool nv)
    {
        UpdateInHandObs(nv);
    }
    [ObserversRpc]
    void UpdateInHandObs(bool nv)
    {
        InHand = nv;
    }
}