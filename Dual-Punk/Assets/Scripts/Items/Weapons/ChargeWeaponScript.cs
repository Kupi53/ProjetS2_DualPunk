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

    private float _chargeTimer;
    public bool isShooting = false;


    private new void Start()
    {
        base.Start();
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

        if (Input.GetButton("Use") && !Reloading && _fireTimer >= _fireRate && _ammoLeft > 0 && _chargeTimer < _chargeTime && !isShooting)
        { 
            if (!AudioManager.Instance.isSpecificSoundPlaying)
            {
                AudioManager.Instance.PlayClipAt(_chargeTimeSound, gameObject.transform.position);
            }
            
            _chargeTimer += Time.deltaTime;
            isShooting = true;
        }
        else if (_fireTimer < _fireRate)
        {
            _fireTimer += Time.deltaTime;
        }
        
        if ((Input.GetButtonUp("Use") || !AudioManager.Instance.isSpecificSoundPlaying) && _chargeTimer > 0 && isShooting)
        {
			AudioManager.Instance.StopSpecificSound();
            
            float multiplier = _chargeTimer / _chargeTime;

            Fire(direction, (int)Methods.GetProgressingFactor(multiplier, _minDamage, _damage), Methods.GetProgressingFactor(multiplier, _minSpeed, _bulletSpeed),
                _dispersion, (int)Methods.GetProgressingFactor(multiplier, _minCollisions, _collisionsAllowed));

            PlayerRecoil.Impact(-direction, Methods.GetProgressingFactor(multiplier, _minRecoil, _recoilForce));
            PlayerState.CameraController.ShakeCamera(Methods.GetProgressingFactor(multiplier, _minCameraShake, _cameraShake), 0.1f);

            _chargeTimer = 0;
            _fireTimer = 0;
            _ammoLeft--;
            
            AudioManager.Instance.PlayClipAt(_fireSound, gameObject.transform.position);

			if (AudioManager.Instance.resetSpecificSoundCoroutine != null)
			{
    			StopCoroutine(AudioManager.Instance.resetSpecificSoundCoroutine);
    			AudioManager.Instance.isSpecificSoundPlaying = false;
				AudioManager.Instance.resetSpecificSoundCoroutine = null;
			}
        }

		if (Input.GetButtonUp("Use"))
		{
			isShooting = false;
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
