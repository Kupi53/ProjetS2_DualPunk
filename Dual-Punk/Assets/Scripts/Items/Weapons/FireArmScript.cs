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

    [SerializeField] private int _damage;
    [SerializeField] private int _maxMagSize;
    [SerializeField] protected int _bulletNumber;
    [SerializeField] private int _reloadAmount;

    [SerializeField] private float _fireRate;
    [SerializeField] private float _dispersion;
    [SerializeField] protected float _aimAccuracy;

    [SerializeField] private bool _autoReload;
    [SerializeField] private bool _auto;

    private float _fireTimer;

    public int CurrentMagSize { get; set; }
    public float ReloadTime { get; set; }
    public float ReloadTimer {  get; set; }
    public bool Reloading { get; set; }


    public void Start()
    {
        ReloadTimer = 0;
        _fireTimer = _fireRate;
        CurrentMagSize = _maxMagSize;
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
        movePosition(position, WeaponOffset, direction, WeaponDistance);

        if ((Input.GetButton("Use") && _auto && !Reloading || Input.GetButtonDown("Use")) && _fireTimer > _fireRate && CurrentMagSize > 0)
        {
            if (Reloading)
                Reset();

            if (IsHost)
                Fire(direction, _dispersion);

            _fireTimer = 0;
            CurrentMagSize--;
        }
        else
            _fireTimer += Time.deltaTime;


        if (Input.GetButtonDown("Reload") && CurrentMagSize != _maxMagSize || _autoReload && CurrentMagSize == 0)
            Reloading = true;

        if (Reloading)
        {
            PointerScript.SpriteNumber = 0;

            if (ReloadTimer >= ReloadTime)
            {
                ReloadTimer = 0;
                if (CurrentMagSize + _reloadAmount < _maxMagSize)
                    CurrentMagSize += _reloadAmount;
                else
                {
                    Reloading = false;
                    CurrentMagSize = _maxMagSize;
                }
            }
            else
                ReloadTimer += Time.deltaTime;
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
        ReloadTimer = 0;
        Reloading = false;
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
            bulletScript.MoveDirection = newDirection;
            bulletScript.transform.eulerAngles = new Vector3(0, 0, newAngle);
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