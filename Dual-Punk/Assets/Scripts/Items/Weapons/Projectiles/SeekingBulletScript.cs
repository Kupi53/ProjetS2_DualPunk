using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;


public class SeekingBulletScript : BulletScript
{
    private float _rotateSpeed;
    
#nullable enable
    public GameObject? Target { get; set; }
#nullable disable


    private new void FixedUpdate()
    {
        if (!IsServer) return;

        base.FixedUpdate();

        if (Target != null)
        {
            Vector3 heading = (Target.transform.position - transform.position).normalized;
            float angle = Vector3.Cross(_moveDirection, heading).z * 100;

            if (angle > _rotateSpeed * _moveFactor)
                angle = _rotateSpeed;
            else if (-angle > _rotateSpeed * _moveFactor)
                angle = -_rotateSpeed;

            ChangeDirection(Quaternion.Euler(0, 0, angle) * _moveDirection, false);
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + angle);
        }
    }


    public void Setup(GameObject target, Vector3 moveDirection, int damage, float moveSpeed, float impactForce, float rotateSpeed, bool damagePlayer, bool warriorLuck, PlayerState playerState)
    {
        base.Setup(moveDirection, damage, moveSpeed, impactForce, 0, damagePlayer, warriorLuck, playerState);

        Target = target;
        _rotateSpeed = rotateSpeed;
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Projectile"))
        {
            if (collider.GetComponent<IDestroyable>().DestroyObject())
                DestroyObject();
        }
        else if (!_stopDamage && (collider.CompareTag("Ennemy") && !_damagePlayer || collider.CompareTag("Player") && _damagePlayer))
        {
            _stopDamage = true;
            collider.GetComponent<IDamageable>().Damage(_damage, 0, _warriorLuck, 0f);
            DestroyObject();
        }
        else if (collider.CompareTag("Wall"))
        {
            DestroyObject();
        }
    }
}
