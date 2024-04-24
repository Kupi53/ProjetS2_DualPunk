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
    [SerializeField] protected int _collisionsAllowed;

    [SerializeField] protected float _fireRate;
    [SerializeField] protected float _dispersion;
    [SerializeField] private float _reloadTime;
    [SerializeField] protected float _aimAccuracy;
    [SerializeField] protected float _bulletSpeed;
    [SerializeField] protected float _bulletSize;

    [SerializeField] protected bool _autoReload;
    [SerializeField] protected bool _isAuto;

    [SerializeField] protected AudioClip _fireSound;
    [SerializeField] protected AudioClip _reloadSound;

    protected int _ammoLeft;
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

        if ((Input.GetButton("Use") && _isAuto && !_reloading || Input.GetButtonDown("Use")) && _fireTimer >= _fireRate && _ammoLeft > 0)
        {
            if (_reloading)
            {
                _reloadTimer = 0;
                _reloading = false;
            }

            Fire(direction, _damage, _bulletSpeed, _bulletSize, _dispersion, _collisionsAllowed);

            PlayerRecoil.Impact(-direction, _recoilForce);
            PlayerState.CameraController.ShakeCamera(_cameraShake, 0.1f);

            AudioManager.Instance.PlayClipAt(_fireSound, gameObject.transform.position);
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
			{
                _ammoLeft += _reloadAmount;
				AudioManager.Instance.PlayClipAt(_reloadSound, gameObject.transform.position);
            }
			else
            {
                _reloading = false;
                _ammoLeft = _magSize;
				AudioManager.Instance.PlayClipAt(_reloadSound, gameObject.transform.position);
            }
        }
        else
            _reloadTimer += Time.deltaTime;
    }


    public override void ResetWeapon()
    {
        _reloadTimer = 0;
        _reloading = false;
    }


    protected virtual void Fire(Vector3 direction, int damage, float bulletSpeed, float bulletSize, float dispersion, int collisionsAllowed)
    {
        _ammoLeft--;
        _fireTimer = 0;

        if (PlayerState.Walking)
            dispersion /= _aimAccuracy;

        FireBulletRpc(direction, damage, bulletSpeed, bulletSize, dispersion, collisionsAllowed);
    }


    [ServerRpc(RequireOwnership = false)]
    private void FireBulletRpc(Vector3 direction, int damage, float bulletSpeed, float bulletSize, float dispersion, int collisionsAllowed)
    {
        for (int i = 0; i < _bulletNumber; i++)
        {
            GameObject newBullet = Instantiate(_bullet, _gunEndPoints[i%_gunEndPoints.Length].transform.position, Quaternion.identity);
            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();
            
            newBullet.transform.localScale = new Vector2(bulletSize, bulletSize);
            Vector3 newDirection = new Vector3(direction.x + Methods.NextFloat(-dispersion, dispersion), direction.y + Methods.NextFloat(-dispersion, dispersion), 0).normalized;
            bulletScript.Setup(damage, bulletSpeed, newDirection, collisionsAllowed);

            Spawn(newBullet);
        }
    }
}