using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChargeWeaponScript : FireArmScript
{
    [SerializeField] private float _minDamage;
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _minRecoil;
    [SerializeField] private float _minImpact;
    [SerializeField] private float _minCameraShake;
    [SerializeField] private float _chargeTime;
    [SerializeField] private int _minCollisions;
    
    [SerializeField] private AudioClip _chargeTimeSound;
	[SerializeField] private AudioSource _audioSource;

    private float _chargeTimer;

    private new void Start()
    {
        base.Start();
		_chargeTime = _chargeTimeSound.length;
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

        if (Input.GetButton("Use") && !Reloading && _fireTimer >= _fireRate && _ammoLeft > 0 && _chargeTimer < _chargeTime)
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
        
        if ((Input.GetButtonUp("Use") || _chargeTimer >= _chargeTime) && _chargeTimer > 0)
        {
			_audioSource.Stop();

            float multiplier = _chargeTimer / _chargeTime;

            Fire(direction, (int)Methods.GetProgressingFactor(multiplier, _minDamage, _damage), Methods.GetProgressingFactor(multiplier, _minSpeed, _bulletSpeed),
                _dispersion, (int)Methods.GetProgressingFactor(multiplier, _minCollisions, _collisionsAllowed));

            PlayerRecoil.Impact(-direction, Methods.GetProgressingFactor(multiplier, _minRecoil, _recoilForce));
            PlayerState.CameraController.ShakeCamera(Methods.GetProgressingFactor(multiplier, _minCameraShake, _cameraShake), 0.1f);

            _chargeTimer = 0;
            _fireTimer = 0;
            _ammoLeft--;

			AudioManager.Instance.PlayClipAt(_fireSound, gameObject.transform.position);
        }

        else if (Input.GetButtonDown("Use") && Reloading && _ammoLeft > 0) {
            Reset();
        }

        if (Input.GetButtonDown("Reload") && _ammoLeft != _magSize || _autoReload && _ammoLeft == 0) {
            Reloading = true;
        } if (Reloading) {
            Reload();
        }      
    }


    public override void ResetWeapon()
    {
        base.Reset();
        _chargeTimer = 0;
    }
}
