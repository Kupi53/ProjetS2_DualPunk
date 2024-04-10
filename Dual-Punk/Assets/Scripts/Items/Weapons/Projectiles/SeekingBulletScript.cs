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
            Debug.Log("arnaud la merde");
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


    public void Setup(int damage, float moveSpeed, Vector3 moveDirection, float rotateSpeed)
    {
        Setup(damage, moveSpeed, moveDirection);
        _rotateSpeed = rotateSpeed;
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Ennemy"))
        {
            EnnemyState health = collider.GetComponent<EnnemyState>();
            health.OnDamage(_damage);
            DestroyThis();
        }
        if (_collisionsAllowed < 0 && collider.CompareTag("Wall"))
        {
            DestroyThis();
        }
    }
}
