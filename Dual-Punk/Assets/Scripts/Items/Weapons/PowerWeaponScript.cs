using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using UnityEngine.Playables;
using Unity.VisualScripting;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;


public class PowerWeaponScript : FireArmScript
{
    [SerializeField] protected AudioClip _reloadSound;
    [SerializeField] protected GameObject _bullet;
    [SerializeField] protected GameObject[] _gunEndPoints;

    [SerializeField] protected bool _isAuto;
    [SerializeField] protected bool _autoReload;
    [SerializeField] protected int _bulletsPerShot;
    [SerializeField] protected int _magSize;
    [SerializeField] private int _reloadAmount;

    [SerializeField] private float _reloadTime;
    [SerializeField] protected float _dispersion;
    [SerializeField] protected float _aimAccuracy;
    [SerializeField] protected float _bulletSpeed;
    [SerializeField] protected float _bulletSize;
    [SerializeField] protected int _bulletCollisions;

    [SerializeField] protected int _ammoLeft;
    protected int _bulletPointIndex;
    private float _reloadTimer;
    protected float _fireTimer;
    protected bool _aiming;
    protected bool _reloading;

    // Nécessaire pour l'implant qui multiplie les dégâts de certaines munitions
    public bool WarriorLuck { get; set; } = false;
    public int DamageMultiplier { get; set; } = 1;
    public int DropPercentage { get; set; } = 1;

    public int AmmoLeft { get => _ammoLeft; set => _ammoLeft = value; }
    public float FireRate { get => _fireRate; set => _fireRate = value; }
    public float ReloadTime { get => _reloadTime; set => _reloadTime = value; }
    public int MagSize { get => _magSize; set => _magSize = value; }
    public int ReloadAmout { get => _reloadAmount; set => _reloadAmount = value; }

    public override bool DisplayInfo { get => _reloading; }
    public override float InfoMaxTime { get => _reloadTime; }
    public override float InfoTimer { get => _reloadTimer; }



    protected void Start()
    {
        _reloadTimer = 0;
        _bulletPointIndex = 0;
        _ammoLeft = _magSize;
        _fireTimer = _fireRate;
    }
    

    public override void Run(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        base.Run(position, direction, targetPoint);

        _aiming = PlayerState.Walking;
        if (_ammoLeft == 0)
            PlayerState.PointerScript.CanShoot = false;


        if (!PlayerState.Stop && !PlayerState.IsDown && (Input.GetButton("Use") && _isAuto && !_reloading || Input.GetButtonDown("Use"))
            && _fireTimer >= _fireRate && _ammoLeft > 0 && _canAttack)
        {
            if (_reloading)
            {
                _reloadTimer = 0;
                _reloading = false;
            }

            Fire(direction, _damage, _dispersion, Vector3.Distance(_gunEndPoints[0].transform.position, targetPoint), false);
            PlayerState.CameraController.ShakeCamera(_cameraShake, 0.1f);
        }

        else if (_fireTimer < _fireRate)
        {
            _fireTimer += Time.deltaTime;
        }

        if (!PlayerState.Stop && !PlayerState.IsDown && (Input.GetButtonDown("Reload") && _ammoLeft != _magSize || _autoReload && _ammoLeft == 0))
        {
            _reloading = true;
        }
        if (_reloading)
        {
            Reload();
        }
    }


    public override void EnemyRun(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        MovePosition(position, direction, targetPoint);

        if (EnemyState.CanAttack && _fireTimer >= _fireRate && !_reloading)
        {
            Fire(direction, _damage, _dispersion, Vector3.Distance(_gunEndPoints[0].transform.position, targetPoint), true);
        }
        else if (_fireTimer < _fireRate)
        {
            _fireTimer += Time.deltaTime;
        }

        _aiming = !EnemyState.Run || !EnemyState.Move;

        if (_ammoLeft == 0)
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

            AudioManager.Instance.PlayClipAt(_reloadSound, gameObject.transform.position, _ownerType);
        }
        else
            _reloadTimer += Time.deltaTime;
    }


    public override void ResetWeapon()
    {
        _reloadTimer = 0;
        _reloading = false;
    }


    protected override void Fire(Vector3 direction, int damage, float dispersion, float distance, bool damagePlayer)
    {
        _ammoLeft--;
        _fireTimer = 0;
        UserRecoil.Impact(-direction, _recoilForce);
        AudioManager.Instance.PlayClipAt(_fireSound, gameObject.transform.position, _ownerType);

        bool warriorLuckBullet = false;
        if (WarriorLuck && UnityEngine.Random.Range(0, 100) < DropPercentage)
        {
            damage *= DamageMultiplier;
            warriorLuckBullet = true;
        }
        if (_aiming)
        {
            dispersion /= _aimAccuracy;
        }
            
        FireBulletRpc(direction, damage, _bulletSpeed, _bulletSize, _impactForce, dispersion, _bulletCollisions, warriorLuckBullet, damagePlayer);
    }


    [ServerRpc(RequireOwnership = false)]
    protected void FireBulletRpc(Vector3 direction, int damage, float bulletSpeed, float bulletSize, float impactForce, float dispersion, int collisionsAllowed, bool warriorLuckBullet, bool damagePlayer)
    {
        for (int i = 0; i < _bulletsPerShot; i++)
        {
            GameObject newBullet = Instantiate(_bullet, _gunEndPoints[_bulletPointIndex].transform.position, Quaternion.identity);
            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();
            
            _bulletPointIndex = (_bulletPointIndex + 1) % _gunEndPoints.Length;
            newBullet.transform.localScale = new Vector2(bulletSize, bulletSize);
            Vector3 newDirection = new Vector3(direction.x + Methods.NextFloat(-dispersion, dispersion), direction.y + Methods.NextFloat(-dispersion, dispersion), 0).normalized;

            bulletScript.Setup(newDirection, damage, bulletSpeed, impactForce, collisionsAllowed, damagePlayer, warriorLuckBullet, PlayerState);
            Spawn(newBullet);
        }
    }
}
