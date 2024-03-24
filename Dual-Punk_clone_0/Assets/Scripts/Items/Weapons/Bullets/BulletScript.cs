using System.Collections;
using System.Collections.Generic;
//using Unity.Netcode;
using UnityEngine;
using System;


public class BulletScript : MonoBehaviour
{
    private Rigidbody2D _rb2d;

    protected float _moveFactor;

    public int Damage { get; set; }
    public float MoveSpeed { get; set; }
    public Vector3 MoveDirection { get; set; }


    private void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
    //    if (!IsServer) return;

        _rb2d.velocity = MoveDirection * MoveSpeed * _moveFactor;

        if (!GetComponent<Renderer>().isVisible)
        {
            Destroy(gameObject);
        }
    }

    public void ChangeDirection(Vector3 direction, bool changeAngle)
    {
        MoveDirection = direction;
        _moveFactor = (float)(Math.Abs(direction.x / 2) + 0.5);

        if (changeAngle)
        {
            float angle = (float)(Math.Atan2(direction.y, direction.x) * (180 / Math.PI));
            transform.eulerAngles = new Vector3(0, 0, angle);
        }

    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Ennemy"))
        {
            EnemyState health = collider.GetComponent<EnemyState>();
            health.OnDamage(Damage);
            Destroy(gameObject);
        }
    }
}