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

    private new void Update()
    {
        base.Update();

        if (InHand && !Reloading)
        {
            PlayerState.PointerScript.SpriteNumber = 3;
        }
    }


    public override void Run(Vector3 position, Vector3 direction)
    {
        MovePosition(position, direction);

        if (Input.GetButton("Use") && !Reloading && _fireTimer >= _fireRate && _ammoLeft > 0 && _chargeTimer <= _chargeTime && !_cancel)
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

        if (_chargeTimer > 0)
        {
            PlayerState.CameraController.ShakeCamera(_cameraShake * _chargeTimer / 3, 0.1f);

            if (!_audioSource.isPlaying)
            {
                ResetWeapon();
            }
        }
        

        if (Input.GetButtonUp("Use") || _chargeTimer > _chargeTime && _isAuto)
        {
            if (_cancel)
            {
                _cancel = false;
            }
            else if (_chargeTimer > 0)
            {
                _audioSource.Stop();

                float multiplier = _chargeTimer / _chargeTime;
                _chargeTimer = 0;

                Fire(direction, (int)GetProgressingFactor(multiplier, _minDamage, _damage), GetProgressingFactor(multiplier, _minSpeed, _bulletSpeed),
                    GetProgressingFactor(multiplier, _minSize, _bulletSize), _dispersion, (int)(multiplier * _collisionsAllowed));

                PlayerRecoil.Impact(-direction, GetProgressingFactor(multiplier, _minRecoil, _recoilForce));
                PlayerState.CameraController.ShakeCamera(_cameraShake, 0.1f);

                AudioManager.Instance.PlayClipAt(_fireSound, gameObject.transform.position);
            }
        }
        else if (Input.GetButtonDown("Use") && Reloading && _ammoLeft > 0)
        {
            base.ResetWeapon();
        }

        if (Input.GetButtonDown("Reload") && _ammoLeft != _magSize || _autoReload && _ammoLeft == 0)
        {
            Reloading = true;
        } 
        if (Reloading)
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
}
