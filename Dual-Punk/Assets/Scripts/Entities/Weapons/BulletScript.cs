using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletScript : MonoBehaviour
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
        rb2d.velocity = MoveDirection * MoveSpeed;

        if (!GetComponent<Renderer>().isVisible)
        {
            Destroy(gameObject);
        }
    }
}