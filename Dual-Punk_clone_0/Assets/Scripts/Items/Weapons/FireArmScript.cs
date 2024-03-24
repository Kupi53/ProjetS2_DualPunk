using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
//using Unity.Netcode;
using UnityEngine.Playables;



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

    private int _ammoLeft;
    private float _reloadTimer;
    protected float _fireTimer;
    private bool _reloading;


    public int AmmoLeft { get => _ammoLeft; set => _ammoLeft = value; }
    public float ReloadTime { get => _reloadTime; set => _reloadTime = value; }
    public float ReloadTimer { get => _reloadTimer; set => _reloadTimer = value; }
    public bool Reloading { get => _reloading; set => _reloading = value; }


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
    }


    public override void Run(Vector3 position, Vector3 direction)
    {
        MovePosition(position, direction, _weaponOffset, _weaponDistance);

        if ((Input.GetButton("Use") && _auto && !_reloading || Input.GetButtonDown("Use")) && _fireTimer >= _fireRate && _ammoLeft > 0)
        {
            if (_reloading)
                Reset();

          //  if (IsHost)
                Fire(direction, _damage, _bulletSpeed, _dispersion);

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


    protected void MovePosition(Vector3 position, Vector3 direction, Vector3 weaponOffset, float weaponDistance)
    {
        float angle = (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));

        if (angle > 90 || angle < -90)
        {
            _spriteRenderer.flipY = true; 
            weaponOffset.x = -weaponOffset.x;
        }
        else
            _spriteRenderer.flipY = false;

        transform.position = position + weaponOffset + direction * weaponDistance;
        transform.eulerAngles = new Vector3(0, 0, angle);
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


    public override void Reset()
    {
        _reloadTimer = 0;
        _reloading = false;
    }


    public virtual void Fire(Vector3 direction, int damage, float bulletSpeed, float dispersion)
    {
        if (PlayerState.Walking)
            dispersion /= _aimAccuracy;

        for (int i = 0; i < _bulletNumber; i++)
        {
            GameObject newBullet = Instantiate(_bullet, _gunEndPoints[i%_gunEndPoints.Length].transform.position, transform.rotation);
            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();

            Vector3 newDirection = new Vector3(direction.x + Methods.NextFloat(-dispersion, dispersion), direction.y + Methods.NextFloat(-dispersion, dispersion), 0).normalized;

            bulletScript.Damage = damage;
            bulletScript.MoveSpeed = bulletSpeed;
            bulletScript.MoveDirection = newDirection;

            bulletScript.ChangeDirection(newDirection, true);

      //      NetworkObject bulletNetwork = newBullet.GetComponent<NetworkObject>();
     //       bulletNetwork.Spawn();
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