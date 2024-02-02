using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletScript : MonoBehaviour
{
    public Vector3 MoveDirection;
    public int damage;
    public int MoveSpeed;

    void Update()
    {
        transform.position += MoveDirection * MoveSpeed * Time.deltaTime;

        if (!GetComponent<Renderer>().isVisible)
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collider){
        if(collider.GetComponent<EnnemyState>()!= null){
            EnnemyState health = collider.GetComponent<EnnemyState>();
            health.OnDamage(damage);
            Destroy(gameObject);
        }
    }
}