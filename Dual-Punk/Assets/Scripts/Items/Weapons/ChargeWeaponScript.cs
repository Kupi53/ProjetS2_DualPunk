using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ChargeWeaponScript : FireArmScript
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
    private float _chargeTimer;


    private new void Start()
    {
        base.Start();

        _cancel = false;
        _chargeTimer = 0;
    }


    public override void Run(Vector3 position, Vector3 direction)
    {
        MovePosition(position, direction);

        if (Input.GetButton("Use") && !_reloading && _fireTimer >= _fireRate && _ammoLeft > 0 && _chargeTimer <= _chargeTime && !_cancel)
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


        if (_chargeTimer > 0 && !_cancel)
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
                    float multiplier = _chargeTimer / _chargeTime;

                    int randomNumber = UnityEngine.Random.Range(0, DropPercentage);

                    if (WarriorLuck && randomNumber == 0)
                    {
                        Fire(direction, (int)GetProgressingFactor(multiplier, _minDamage, _damage * DamageMultiplier), GetProgressingFactor(multiplier, _minSpeed, _bulletSpeed),
                            GetProgressingFactor(multiplier, _minSize, _bulletSize), GetProgressingFactor(multiplier, _minImpact, _impactForce),
                            _dispersion, (int)(multiplier * _collisionsAllowed));
                            
                        _warriorLuckBullet = true;
                    }
                    else
                    {
                        Fire(direction, (int)GetProgressingFactor(multiplier, _minDamage, _damage), GetProgressingFactor(multiplier, _minSpeed, _bulletSpeed),
                            GetProgressingFactor(multiplier, _minSize, _bulletSize), GetProgressingFactor(multiplier, _minImpact, _impactForce),
                            _dispersion, (int)(multiplier * _collisionsAllowed));
                    }

                    PlayerRecoil.Impact(-direction, GetProgressingFactor(multiplier, _minRecoil, _recoilForce));
                    PlayerState.CameraController.ShakeCamera(_cameraShake, 0.1f);
                }
            }
        }
        else if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }

        
        if (Input.GetButtonDown("Use"))
        {
            _cancel = false;
            if (_reloading && _ammoLeft > 0)
                base.ResetWeapon();
        }

        if (Input.GetButtonDown("Reload") && _ammoLeft != _magSize && _chargeTimer == 0 || _autoReload && _ammoLeft == 0)
        {
            _reloading = true;
        } 
        if (_reloading)
        {
            Reload();
        }
    }


    public static float GetProgressingFactor(float multiplier, float minValue, float maxValue)
    {
        return minValue + multiplier * (maxValue - minValue);
    }


    public override void ResetWeapon()
    {
        base.ResetWeapon();

        _cancel = true;
        _fireTimer = 0;
        _chargeTimer = 0;
        _audioSource.Stop();
    }


    public override void Fire(Vector3 direction, int damage, float bulletSpeed, float bulletSize, float impactForce, float dispersion, int collisionsAllowed)
    {
        _chargeTimer = 0;

        base.Fire(direction, damage, bulletSpeed, bulletSize, impactForce, dispersion, collisionsAllowed);
    }
}