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
    protected int _bulletPointIndex;
    private float _reloadTimer;
    protected float _fireTimer;

    public int AmmoLeft { get => _ammoLeft; set => _ammoLeft = value; }
    public override bool DisplayInfo { get => _reloading; }
    public override float InfoMaxTime { get => _reloadTime; }
    public override float InfoTimer { get => _reloadTimer; }


    protected new void Start()
    {
        base.Start();

        _reloadTimer = 0;
        _bulletPointIndex = 0;
        _ammoLeft = _magSize;
        _fireTimer = _fireRate;
    }

    protected new void Update()
    {
        base.Update();

        if (!InHand) return;

        if (_canAttack && _ammoLeft > 0)
            PlayerState.PointerScript.CanShoot = true;
        else
            PlayerState.PointerScript.CanShoot = false;
    }


    public override void Run(Vector3 position, Vector3 direction)
    {
        MovePosition(position, direction);

        if ((Input.GetButton("Use") && _isAuto && !_reloading || Input.GetButtonDown("Use")) && _fireTimer >= _fireRate && _ammoLeft > 0 && _canAttack)
        {
            if (_reloading)
            {
                _reloadTimer = 0;
                _reloading = false;
            }

            Fire(direction, _damage, _bulletSpeed, _bulletSize, _dispersion, _collisionsAllowed);

            PlayerRecoil.Impact(-direction, _recoilForce);
            PlayerState.CameraController.ShakeCamera(_cameraShake, 0.1f);
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
        if (_reloadTimer >= _reloadTime)
        {
            _reloadTimer = 0;

            if (_ammoLeft + _reloadAmount < _magSize)
			{
                _ammoLeft += _reloadAmount;
            }
			else
            {
                _reloading = false;
                _ammoLeft = _magSize;
            }

            AudioManager.Instance.PlayClipAt(_reloadSound, gameObject.transform.position);
        }
        else
            _reloadTimer += Time.deltaTime;
    }


    public override void ResetWeapon()
    {
        _reloadTimer = 0;
        _reloading = false;
    }


    public virtual void Fire(Vector3 direction, int damage, float bulletSpeed, float bulletSize, float dispersion, int collisionsAllowed)
    {
        _ammoLeft--;
        _fireTimer = 0;
        AudioManager.Instance.PlayClipAt(_fireSound, gameObject.transform.position);

        if (PlayerState.Walking)
            dispersion /= _aimAccuracy;

        FireBulletRpc(direction, damage, bulletSpeed, bulletSize, dispersion, collisionsAllowed);
    }


    [ServerRpc(RequireOwnership = false)]
    private void FireBulletRpc(Vector3 direction, int damage, float bulletSpeed, float bulletSize, float dispersion, int collisionsAllowed)
    {
        for (int i = 0; i < _bulletNumber; i++)
        {
            GameObject newBullet = Instantiate(_bullet, _gunEndPoints[_bulletPointIndex].transform.position, Quaternion.identity);
            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();

            _bulletPointIndex = (_bulletPointIndex + 1) % _gunEndPoints.Length;
            newBullet.transform.localScale = new Vector2(bulletSize, bulletSize);
            Vector3 newDirection = new Vector3(direction.x + Methods.NextFloat(-dispersion, dispersion), direction.y + Methods.NextFloat(-dispersion, dispersion), 0).normalized;

            bulletScript.Setup(damage, bulletSpeed, newDirection, collisionsAllowed);
            Spawn(newBullet);
        }
    }
}