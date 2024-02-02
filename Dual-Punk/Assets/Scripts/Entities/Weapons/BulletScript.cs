using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletScript : MonoBehaviour
{
    public Vector3 MoveDirection;
    public int MoveSpeed;

    void Update()
    {
        transform.position += MoveDirection * MoveSpeed * Time.deltaTime;

        if (!GetComponent<Renderer>().isVisible)
        {
            Destroy(gameObject);
        }
    }
}