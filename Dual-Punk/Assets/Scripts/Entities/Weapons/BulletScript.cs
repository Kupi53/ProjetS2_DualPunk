using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class BulletScript : NetworkBehaviour
{
    internal Rigidbody2D rb2d;

    public Vector3 MoveDirection;
    public int MoveSpeed;


    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        if (!IsServer) return;
        rb2d.velocity = MoveDirection * MoveSpeed;
        if (!GetComponent<Renderer>().isVisible)
        {
            Destroy(gameObject);
        }
    }
}