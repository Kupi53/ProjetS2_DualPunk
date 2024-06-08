using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ChargeWeaponScript : PowerWeaponScript
{
    [SerializeField] private float _chargeTime;
    [SerializeField] private float _minDamage;
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _minSize;
    [SerializeField] private float _minRecoil;
    [SerializeField] private float _minImpact;
    
    [SerializeField] private AudioClip _chargeTimeSound;
	[SerializeField] private AudioSource _audioSource;

    private bool _cancel;
    private float _minCharge;
    private float _chargeTimer;

    public bool ChargeMax { get; set; }


    private new void Start()
    {
        base.Start();

        _cancel = false;
        _minCharge = 0;
        _chargeTimer = 0;
    }


    public override void Run(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        MovePosition(position, direction, targetPoint);

        _aiming = PlayerState.Walking;
        PlayerState.PointerScript.SpriteNumber = _pointerSpriteNumber;

        if (!_canAttack || _ammoLeft == 0)
            PlayerState.PointerScript.CanShoot = false;
        else
            PlayerState.PointerScript.CanShoot = true;


        if (Input.GetButton("Use") && !_reloading && _fireTimer >= _fireRate && _ammoLeft > 0 && _chargeTimer <= _chargeTime && !_cancel && !PlayerState.Stop)
        {
			if (!_audioSource.isPlaying)
			{
				_audioSource.clip = _chargeTimeSound;
				_audioSource.Play();
			}

            _chargeTimer += Time.deltaTime;
		}
        else if (_fireTimer < _fireRate)
        {
            _fireTimer += Time.deltaTime;
        }


        if (_chargeTimer > 0 && !_cancel && !PlayerState.Stop)
        {
            PlayerState.CameraController.ShakeCamera(_cameraShake * _chargeTimer / 3, 0.1f);

            if (!_audioSource.isPlaying)
            {
                ResetWeapon();
            }

            if (Input.GetButtonUp("Use") || _chargeTimer > _chargeTime && _isAuto)
            {
                if (!_canAttack)
                {
                    ResetWeapon();
                }
                else
                {
                    _audioSource.Stop();

                    Fire(direction, _damage, _dispersion, 0, false);
                    PlayerState.CameraController.ShakeCamera(_cameraShake, 0.1f);
                }
            }
        }

        else if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
        
        if (Input.GetButtonDown("Use") && !PlayerState.Stop)
        {
            _cancel = false;
            if (_reloading && _ammoLeft > 0)
                base.ResetWeapon();
        }

        if (Input.GetButtonDown("Reload") && _ammoLeft != _magSize && _chargeTimer == 0 && !PlayerState.Stop || _autoReload && _ammoLeft == 0)
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
            if (_chargeTimer == 0)
            {
                if (ChargeMax)
                {
                    _minCharge = 0.9f * _chargeTime;
                }
                else
                {
                    _minCharge = UnityEngine.Random.Range(0, _chargeTime);
                }
            }

            _chargeTimer += Time.deltaTime;
        }
        else if (_chargeTimer > 0)
        {
            _minCharge = 0;
            _chargeTimer = 0;
        }
        else if (_fireTimer < _fireRate)
        {
            _fireTimer += Time.deltaTime;
        }

        if (_chargeTimer > _minCharge)
        {
            _minCharge = 0;
            Fire(direction, _damage, _dispersion, 0, true);
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


    private float GetProgressingFactor(float multiplier, float minValue, float maxValue)
    {
        return minValue + multiplier * (maxValue - minValue);
    }


    public override void ResetWeapon()
    {
        base.ResetWeapon();

        _cancel = true;
        _minCharge = 0;
        _fireTimer = 0;
        _chargeTimer = 0;
        _audioSource.Stop();
    }


    protected override void Fire(Vector3 direction, int damage, float dispersion, float distance, bool damagePlayer)
    {
        bool warriorLuckBullet = false;
        float multiplier = _chargeTimer / _chargeTime;

        _ammoLeft--;
        _fireTimer = 0;
        _chargeTimer = 0;

        if (PlayerState != null)
            AudioManager.Instance.PlayClipAt(_fireSound, gameObject.transform.position, "Player");
        else
            AudioManager.Instance.PlayClipAt(_fireSound, gameObject.transform.position, "Enemy");
        UserRecoil.Impact(-direction, GetProgressingFactor(multiplier, _minRecoil, _recoilForce));

        if (WarriorLuck && UnityEngine.Random.Range(0, 100) < DropPercentage)
        {
            damage *= DamageMultiplier;
            warriorLuckBullet = true;
        }

        FireBulletRpc(direction, (int)GetProgressingFactor(multiplier, _minDamage, damage), GetProgressingFactor(multiplier, _minSpeed, _bulletSpeed),
                      GetProgressingFactor(multiplier, _minSize, _bulletSize), GetProgressingFactor(multiplier, _minImpact, _impactForce),
                      _dispersion, (int)(multiplier * _bulletCollisions), warriorLuckBullet, damagePlayer);
    }
}