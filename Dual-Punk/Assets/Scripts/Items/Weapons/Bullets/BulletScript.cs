using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class BulletScript : NetworkBehaviour
{
    private Rigidbody2D _rb2d;

    public int Damage { get; set; }
    public float MoveSpeed { get; set; }
    public Vector3 MoveDirection {  get; set; }


    private void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        if (!IsServer) return;

        _rb2d.velocity = MoveDirection * MoveSpeed;

        if (!GetComponent<Renderer>().isVisible)
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.GetComponent<EnnemyState>()!= null)
        {
            EnnemyState health = collider.GetComponent<EnnemyState>();
            health.OnDamage(Damage);
            Destroy(gameObject);
        }
    }
}