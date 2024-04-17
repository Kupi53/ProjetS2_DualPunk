using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FishNet.Object;


public class BulletScript : NetworkBehaviour, IImpact
{
    [SerializeField] protected float _lifeTime;

    protected Rigidbody2D _rb2d;
    protected Vector3 _moveDirection;

    protected int _damage;
    protected int _collisionsAllowed;
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

        _lifeTime -= Time.fixedDeltaTime;
        _rb2d.velocity = _moveDirection * _moveSpeed * _moveFactor;

        if (_lifeTime <= 0 || _moveSpeed < 5)
        {
            Destroy(gameObject);
        }
    }


    public void Setup(int damage, float moveSpeed, Vector3 moveDirection, int collisionsAllowed)
    {
        _damage = damage;
        _moveSpeed = moveSpeed;
        _collisionsAllowed = collisionsAllowed;

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


    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ennemy"))
        {
            EnnemyState health = collision.collider.GetComponent<EnnemyState>();
            health.OnDamage(_damage);
            DestroyThis();
        }
        else if (collision.collider.CompareTag("Wall"))
        {
            _collisionsAllowed--;
            Vector2 reflectDirection = Vector2.Reflect(_moveDirection, collision.contacts[0].normal);
            _rb2d.transform.position = collision.contacts[0].point + reflectDirection * ((Vector2)transform.position - collision.contacts[0].point).magnitude * 2;
            ChangeDirection(Vector2.Reflect(_moveDirection, collision.contacts[0].normal), true);
        }

        if (_collisionsAllowed < 0 || collision.collider.CompareTag("Player"))
        {
            DestroyThis();
        }
    }
}