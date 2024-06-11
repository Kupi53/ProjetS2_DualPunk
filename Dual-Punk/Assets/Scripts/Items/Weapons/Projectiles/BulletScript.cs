using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FishNet.Object;


public class BulletScript : NetworkBehaviour, IImpact, IDestroyable
{
    [SerializeField] protected float _lifeTime;

    protected Rigidbody2D _rb2d;
    protected Vector3 _moveDirection;

    protected int _damage;
    protected int _collisionsAllowed;
    protected float _moveSpeed;
    protected float _moveFactor;
    protected float _impactForce;
    protected bool _damagePlayer;
    protected bool _warriorLuck;
    protected bool _stopDamage;

    protected void Start()
    {
        _stopDamage = false;
        _rb2d = GetComponent<Rigidbody2D>();
    }


    protected void FixedUpdate()
    {
        if (!IsServer) return;

        _lifeTime -= Time.fixedDeltaTime;
        _rb2d.velocity = _moveDirection * _moveSpeed * _moveFactor;

        if (_lifeTime <= 0 || _moveSpeed < 5)
        {
            DestroyObject();
        }
    }


    public void Setup(Vector3 moveDirection, int damage, float moveSpeed, float impactForce, int collisionsAllowed, bool damagePlayer, bool warriorLuck)
    {
        _damage = damage;
        _moveSpeed = moveSpeed;
        _impactForce = impactForce;
        _collisionsAllowed = collisionsAllowed;
        _damagePlayer = damagePlayer;
        _warriorLuck = warriorLuck;

        if (warriorLuck)
        {
            GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 255);
        }

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
        Vector3 newDirection = _moveDirection + (Vector3)direction.normalized * intensity;
        _moveSpeed *= newDirection.magnitude;

        ChangeDirection(newDirection, true);
    }

    public virtual bool DestroyObject()
    {
        Destroy(gameObject);
        return true;
    }


    protected void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collider = collision.collider.gameObject;

        if (collider.CompareTag("Wall"))
        {
            _collisionsAllowed--;
            _damage = (int)(_damage * 0.75f);

            Vector2 reflectDirection = Vector2.Reflect(_moveDirection, collision.contacts[0].normal);
            float magnitude = ((Vector2)transform.position - collision.contacts[0].point).magnitude;
            _rb2d.transform.position = collision.contacts[0].point + reflectDirection * magnitude * 3 + (Vector2)_moveDirection * magnitude;
            ChangeDirection(Vector2.Reflect(_moveDirection, collision.contacts[0].normal), true);
        }

        if (_collisionsAllowed < 0 || collider.CompareTag("Player") && !_damagePlayer || collider.CompareTag("Ennemy") && _damagePlayer)
        {
            DestroyObject();
        }
    }

    protected void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            DestroyObject();
        }
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
            collider.GetComponent<IImpact>().Impact(_moveDirection, _impactForce);
            DestroyObject();
        }
    }
}