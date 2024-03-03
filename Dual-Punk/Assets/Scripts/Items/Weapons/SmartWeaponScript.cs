using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using System;
using Unity.Netcode;


public class SmartWeaponScript : FireArmScript
{
    [SerializeField] private float _bulletRotateSpeed;

    private new void Start()
    {
        base.Start();
    }

    private new void Update()
    {
        base.Update();

        if (InHand && !_reloading)
        {
            PointerScript.SpriteNumber = 2;

            if (PointerScript.Target != null && Input.GetButtonDown("Switch"))
            {
                PointerScript.Locked = !PointerScript.Locked;
            }
        }
    }


    public override void Fire(Vector3 direction, float spread)
    {
        if (PlayerState.Walking)
            spread /= _aimAccuracy;

        for (int i = 0; i < _bulletNumber; i++)
        {
            Vector3 newDirection = new Vector3(direction.x + NextFloat(-spread, spread), direction.y + NextFloat(-spread, spread), 0).normalized;
            float newAngle = (float)(Math.Atan2(newDirection.y, newDirection.x) * (180 / Math.PI));

            GameObject newBullet = Instantiate(_bullet, _gunEnd.transform.position, transform.rotation);
            SeekingBulletScript bulletScript = newBullet.GetComponent<SeekingBulletScript>();

            bulletScript.Damage = _damage;
            bulletScript.MoveSpeed = _bulletSpeed;
            bulletScript.MoveDirection = newDirection;
            bulletScript.Target = PointerScript.Target;
            bulletScript.RotateSpeed = _bulletRotateSpeed;

            newBullet.transform.eulerAngles = new Vector3(0, 0, newAngle);
            NetworkObject bulletNetwork = newBullet.GetComponent<NetworkObject>();
            bulletNetwork.Spawn();
        }
    }
}