using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChargedBulletScript : BulletScript
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Projectile"))
        {
            collider.GetComponent<IDestroyable>().Destroy();
            Destroy(gameObject);
        }
        if (collider.CompareTag("Ennemy"))
        {
            EnnemyState health = collider.GetComponent<EnnemyState>();
            health.OnDamage(_damage);
            Destroy(gameObject);
        }
        else if (collider.CompareTag("Wall"))
        {
            _collisionsAllowed--;
            _damage = (int)(_damage * 0.75f);

            if (_collisionsAllowed < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}