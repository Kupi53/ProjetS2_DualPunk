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
    public abstract void Drop();

    protected void MovePosition(Vector3 position, Vector3 direction)
    {
        if (PlayerState.MousePosition.x < PlayerState.transform.position.x)
        {
            if (!_spriteRenderer.flipY)
            {
                _weaponOffset.x = -_weaponOffset.x;
                _spriteRenderer.flipY = true;
            }
        }
        else
        {
            if (_spriteRenderer.flipY)
            {
                _weaponOffset.x = -_weaponOffset.x;
                _spriteRenderer.flipY = false;
            }
        }

        transform.position = position + _weaponOffset + direction * _weaponDistance;
        transform.eulerAngles = new Vector3(0, 0, Methods.GetAngle(direction));
    }


    void Start(){
        Debug.Log("testtt");
        if (IsServer){
            Debug.Log("test");
            Spawn(gameObject);
        }

    }

    void Awake(){
        _objectSpawner = GameObject.Find("ObjectSpawner").GetComponent<ObjectSpawner>();
    }
}