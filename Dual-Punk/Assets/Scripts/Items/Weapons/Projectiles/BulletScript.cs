using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;


public class BulletScript : NetworkBehaviour, IImpact
{
    protected Rigidbody2D _rb2d;
    protected Vector3 _moveDirection;
    protected int _damage;
    protected float _moveSpeed;
    protected float _moveFactor;
    protected float _impactForce;


    private void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Impact(Vector2.up, 0.5f);
        }
    }

    protected void FixedUpdate()
    {
        if (!IsServer) return;

        // Marche pas avec MovePosition
        _rb2d.velocity = _moveDirection * _moveSpeed * _moveFactor;

        if (!GetComponent<Renderer>().isVisible || _moveSpeed < 5)
        {
            DestroyThis();
        }
    }


    public void Setup(int damage, float moveSpeed, Vector3 moveDirection)
    {
        _damage = damage;
        _moveSpeed = moveSpeed;

        ChangeDirection(moveDirection, true);
    }


    protected void ChangeDirection(Vector3 direction, bool changeAngle)
    {
        _moveDirection = direction.normalized;
        _moveFactor = Methods.GetDirectionFactor(direction);

        if (changeAngle)
        {
            float angle = Methods.GetAngle(direction);
            transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }


    public void Impact(Vector2 direction, float intensity)
    {
        Vector2 newDirection = _moveDirection + (Vector3)direction * intensity;
        _moveSpeed *= newDirection.magnitude;

        ChangeDirection(newDirection, true);
    }


    protected virtual void DestroyThis()
    {
        Destroy(gameObject);
    }


    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Ennemy"))
        {
            EnnemyState health = collider.GetComponent<EnnemyState>();
            health.OnDamage(_damage);
        }
        if (!collider.CompareTag("Weapon") && !collider.CompareTag("Projectile") && !collider.CompareTag("UI"))
        {
            DestroyThis();
        }
    }
}