using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using UnityEngine.Playables;
using Unity.VisualScripting;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;


public class FireArmScript : WeaponScript
{
    [SerializeField] protected GameObject _bullet;
    [SerializeField] protected GameObject[] _gunEndPoints;

    [SerializeField] protected int _damage;
    [SerializeField] protected int _magSize;
    [SerializeField] protected int _bulletNumber;
    [SerializeField] private int _reloadAmount;

    [SerializeField] protected float _fireRate;
    [SerializeField] protected float _dispersion;
    [SerializeField] private float _reloadTime;
    [SerializeField] protected float _aimAccuracy;
    [SerializeField] protected float _bulletSpeed;

    [SerializeField] protected bool _autoReload;
    [SerializeField] protected bool _auto;

    [SerializeField] protected AudioClip _sound;

    private int _ammoLeft;
    private float _reloadTimer;
    protected float _fireTimer;
    private bool _reloading;

    public int AmmoLeft { get => _ammoLeft; set => _ammoLeft = value; }
    public bool Reloading { get => _reloading; set => _reloading = value; }

    public override bool DisplayInfo { get => _reloading; }
    public override float InfoMaxTime { get => _reloadTime; }
    public override float InfoTimer { get => _reloadTimer; }


    public void Start()
    {
        _reloadTimer = 0;
        _reloading = false;
        _ammoLeft = _magSize;
        _fireTimer = _fireRate;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }


    public void Update()
    {
        //Faire des animations ici
        if (InHand)
        {
            PlayerState.PointerScript.SpriteNumber = 1;
        }
        else
        {
            transform.position += Vector3.up * (float)Math.Cos(Time.time * 5 + _damage) * 0.001f;
        }
    }


    public override void Run(Vector3 position, Vector3 direction)
    {
        MovePosition(position, direction);

        if ((Input.GetButton("Use") && _auto && !_reloading || Input.GetButtonDown("Use")) && _fireTimer >= _fireRate && _ammoLeft > 0)
        {
            if (_reloading)
            {
                _reloadTimer = 0;
                _reloading = false;
            }

            Fire(direction, _damage, _bulletSpeed, _dispersion);
            PlayerRecoil.Impact(-direction, _recoilForce);
            PlayerState.CameraController.ShakeCamera(_cameraShake, 0.1f);

            _fireTimer = 0;
            _ammoLeft--;
        }
        else if (_fireTimer < _fireRate)
        {
            _fireTimer += Time.deltaTime;
        }


        if (Input.GetButtonDown("Reload") && _ammoLeft != _magSize || _autoReload && _ammoLeft == 0)
        {
            _reloading = true;
        }
        if (_reloading)
        {
            Reload();
        }   
    }


    protected void Reload()
    {
        PlayerState.PointerScript.SpriteNumber = 0;

        if (_reloadTimer >= _reloadTime)
        {
            _reloadTimer = 0;

            if (_ammoLeft + _reloadAmount < _magSize)
                _ammoLeft += _reloadAmount;
            else
            {
                _reloading = false;
                _ammoLeft = _magSize;
            }
        }
        else
            _reloadTimer += Time.deltaTime;
    }


    public override void ResetWeapon()
    {
        _reloadTimer = 0;
        _reloading = false;
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }


    public virtual void Fire(Vector3 direction, int damage, float bulletSpeed, float dispersion)
    {
        FireBulletRpc(_bullet, transform.rotation, direction, damage, bulletSpeed, dispersion, PlayerState);
        AudioManager.Instance.PlayClipAt(_sound, gameObject.transform.position);
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void FireBulletRpc(GameObject bullet, Quaternion rot, Vector3 direction, int damage, float bulletSpeed, float dispersion, PlayerState playerState){
        if (playerState.Walking)
            dispersion /= _aimAccuracy;
        for (int i = 0; i < _bulletNumber; i++)
        {
            GameObject newBullet = Instantiate(bullet, _gunEndPoints[i%_gunEndPoints.Length].transform.position, rot);
            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();

            Vector3 newDirection = new Vector3(direction.x + Methods.NextFloat(-dispersion, dispersion), direction.y + Methods.NextFloat(-dispersion, dispersion), 0).normalized;
            bulletScript.Setup(damage, bulletSpeed, newDirection);

            Spawn(newBullet);
        }
    }

    [ServerRpc]
    void RemoveAllOwnerShipRPC(NetworkObject networkObject){
        networkObject.RemoveOwnership();   
    }
}