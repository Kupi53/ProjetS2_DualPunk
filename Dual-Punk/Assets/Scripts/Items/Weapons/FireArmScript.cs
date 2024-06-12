using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;


public abstract class FireArmScript : WeaponScript
{
    [SerializeField] protected bool _silencer;
    [SerializeField] protected float _fireRate;
    [SerializeField] protected AudioClip _fireSound;

    protected bool _canAttack;


    private new void Awake()
    {
        base.Awake();

        _canAttack = true;
    }


    public override void Run(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        MovePosition(position, direction, targetPoint);

        PlayerState.PointerScript.SpriteNumber = _pointerSpriteNumber;
        if (!_canAttack)
            PlayerState.PointerScript.CanShoot = false;
        else
            PlayerState.PointerScript.CanShoot = true;
    }

    public override void MovePosition(Vector3 position, Vector3 direction, Vector3 targetPoint)
    {
        if (Math.Sign(targetPoint.x - position.x) != Math.Sign(transform.localScale.y))
        {
            transform.localScale = new Vector2(transform.localScale.x, -transform.localScale.y);
            WeaponOffset = new Vector3(-WeaponOffset.x, _weaponOffset.y, 0);
        }

        transform.position = position + WeaponOffset + direction * _weaponDistance;
        transform.eulerAngles = new Vector3(0, 0, Methods.GetAngle(direction));
    }


    protected abstract void Fire(Vector3 direction, int damage, float dispersion, float distance, bool damagePlayer);


    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Wall"))
        {
            _canAttack = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Wall"))
        {
            _canAttack = true;
        }
    }
}
