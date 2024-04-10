using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChargedBulletScript : BulletScript
{

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Ennemy"))
        {
            EnnemyState health = collider.GetComponent<EnnemyState>();
            health.OnDamage(_damage);
            DestroyThis();
        }
        else if (collider.CompareTag("Wall"))
        {
            _collisionsAllowed--;
        }

        if (_collisionsAllowed < 0 && collider.CompareTag("Wall"))
        {
            DestroyThis();
        }
    }
}