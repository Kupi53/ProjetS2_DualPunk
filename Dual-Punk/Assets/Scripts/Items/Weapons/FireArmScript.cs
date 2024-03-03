using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using Unity.Netcode;
using UnityEngine.Playables;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;


public class FireArmScript : WeaponScript
{
    [SerializeField] protected GameObject _bullet;
    [SerializeField] protected GameObject _gunEnd;

    [SerializeField] protected int _damage;
    [SerializeField] private int _maxMagSize;
    [SerializeField] protected int _bulletNumber;
    [SerializeField] private int _reloadAmount;

    [SerializeField] private float _fireRate;
    [SerializeField] private float _dispersion;
    [SerializeField] private float _reloadTime;
    [SerializeField] protected float _aimAccuracy;
    [SerializeField] protected float _bulletSpeed;

    [SerializeField] private bool _autoReload;
    [SerializeField] private bool _auto;

    private int _currentMagSize;
    private float _reloadTimer;
    private float _fireTimer;
    protected bool _reloading;

    
    public int CurrentMagSize { get => _currentMagSize; set => _currentMagSize = value; }
    public float ReloadTime { get => _reloadTime; set => _reloadTime = value; }
    public float ReloadTimer {  get => _reloadTimer; set => _reloadTimer = value; }
    public bool Reloading { get => _reloading; set => _reloading = value; }


    public void Start()
    {
        _reloadTimer = 0;
        _reloading = false;
        _fireTimer = _fireRate;
        _currentMagSize = _maxMagSize;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }


    public void Update()
    {
        //Faire des animations ici
        if (InHand)
        {
            PointerScript = PlayerState.Pointer.GetComponent<PointerScript>();
            PointerScript.SpriteNumber = 1;
        }
        else
            PointerScript = null;
    }


    public override void Run(Vector3 position, Vector3 direction)
    {
        movePosition(position, _weaponOffset, direction, _weaponDistance);

        if ((Input.GetButton("Use") && _auto && !_reloading || Input.GetButtonDown("Use")) && _fireTimer > _fireRate && _currentMagSize > 0)
        {
            if (_reloading)
                Reset();

            if (IsHost)
                Fire(direction, _dispersion);

            _fireTimer = 0;
            _currentMagSize--;
        }
        else
            _fireTimer += Time.deltaTime;


        if (Input.GetButtonDown("Reload") && _currentMagSize != _maxMagSize || _autoReload && _currentMagSize == 0)
            _reloading = true;

        if (_reloading)
        {
            PointerScript.SpriteNumber = 0;

            if (_reloadTimer >= _reloadTime)
            {
                _reloadTimer = 0;
                if (_currentMagSize + _reloadAmount < _maxMagSize)
                    _currentMagSize += _reloadAmount;
                else
                {
                    _reloading = false;
                    _currentMagSize = _maxMagSize;
                }
            }
            else
                _reloadTimer += Time.deltaTime;
        }   
    }

    public void movePosition(Vector3 position, Vector3 weaponOffset, Vector3 direction, float weaponDistance)
    {
        float angle = (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));
        if (angle > 90 || angle < -90)
            _spriteRenderer.flipY = true;
        else
            _spriteRenderer.flipY = false;
        transform.position = position + weaponOffset + direction * weaponDistance;
        transform.eulerAngles = new Vector3(0, 0, angle);
    }


    public override void Reset()
    {
        _reloadTimer = 0;
        _reloading = false;
    }


    public static float NextFloat(float min, float max)
    {
        System.Random random = new System.Random();
        double val = random.NextDouble() * (max - min) + min;
        return (float)val;
    }

    public virtual void Fire(Vector3 direction, float spread)
    {
        if (PlayerState.Walking)
            spread /= _aimAccuracy;

        for (int i = 0; i < _bulletNumber; i++)
        {
            Vector3 newDirection = new Vector3(direction.x + NextFloat(-spread, spread), direction.y + NextFloat(-spread, spread), 0).normalized;
            float newAngle = (float)(Math.Atan2(newDirection.y, newDirection.x) * (180 / Math.PI));

            GameObject newBullet = Instantiate(_bullet, _gunEnd.transform.position, transform.rotation);
            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();

            bulletScript.Damage = _damage;
            bulletScript.MoveSpeed = _bulletSpeed;
            bulletScript.MoveDirection = newDirection;

            newBullet.transform.eulerAngles = new Vector3(0, 0, newAngle);

            NetworkObject bulletNetwork = newBullet.GetComponent<NetworkObject>();
            bulletNetwork.Spawn();
        }
    }



    /*public void FireRound(GameObject bullet, GameObject gunEnd, Vector3 direction, float dispersion, int bulletNumber, float aimAccuracy)
    {
        if (playerState.Walking)
            dispersion /= aimAccuracy;

        for (int i = 0; i < bulletNumber; i++)
        {
            Vector3 newDirection = new Vector3(direction.x + NextFloat(-dispersion,dispersion), direction.y + NextFloat(-dispersion, dispersion), 0);
            float newAngle = (float)(Math.Atan2(newDirection.y, newDirection.x) * (180 / Math.PI));

            GameObject newBullet = Instantiate(bullet, gunEnd.transform.position, transform.rotation);
            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();
            bulletScript.MoveDirection = newDirection;
            bulletScript.transform.eulerAngles = new Vector3(0, 0, newAngle);
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void FireRoundServerRPC(NetworkObjectReference bulletRef, NetworkObjectReference gunEndRef, Vector3 direction, float dispersion, int bulletNumber, float aimAccuracy, ulong clientId)
    {
        if (playerState.Walking)
            dispersion /= aimAccuracy;

        for (int i = 0; i < bulletNumber; i++)
        {
            bulletRef.TryGet(out NetworkObject netBullet);
            GameObject bullet = netBullet.gameObject;
            gunEndRef.TryGet(out NetworkObject netGunEnd);
            GameObject gunEnd = netGunEnd.gameObject;
            Vector3 newDirection = new Vector3(direction.x + NextFloat(-dispersion, dispersion), direction.y + NextFloat(-dispersion, dispersion), 0);
            float newAngle = (float)(Math.Atan2(newDirection.y, newDirection.x) * (180 / Math.PI));
            GameObject newBullet = Instantiate(bullet, gunEnd.transform.position, transform.rotation);
            newBullet.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();
            bulletScript.MoveDirection = newDirection;
            bulletScript.transform.eulerAngles = new Vector3(0, 0, newAngle);
        }
    }*/
}