using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChargeWeaponScript : FireArmScript
{
    [SerializeField] private float _minDamage;
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _chargeTime;

    private float _chargeTimer;


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
        MovePosition(position, direction, _weaponOffset, _weaponDistance);

        if (Input.GetButton("Use") && !Reloading && _fireTimer >= _fireRate && AmmoLeft > 0 && _chargeTimer < _chargeTime) {
            _chargeTimer += Time.deltaTime;
        }
        else if (_fireTimer < _fireRate) {
            _fireTimer += Time.deltaTime;
        }

        if (Input.GetButtonUp("Use") && _chargeTimer > 0) 
        {
           // if (IsHost)
            {
                float multiplier = _chargeTimer / _chargeTime;
                Fire(direction, (int)(multiplier * (_damage - _minDamage) + _minDamage), multiplier * (_bulletSpeed - _minSpeed) + _minSpeed, _dispersion);
            }

            _chargeTimer = 0;
            _fireTimer = 0;
            AmmoLeft--;
        }

        else if (Input.GetButtonDown("Use") && Reloading && AmmoLeft > 0) {
            Reset();
        }


        if (Input.GetButtonDown("Reload") && AmmoLeft != _magSize || _autoReload && AmmoLeft == 0) {
            Reloading = true;
        }
        if (Reloading) {
            Reload();
        }      
    }


    public override void Reset()
    {
        base.Reset();
        _chargeTimer = 0;
    }
}
