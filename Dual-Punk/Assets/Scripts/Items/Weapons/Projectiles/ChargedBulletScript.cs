using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChargedBulletScript : BulletScript
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Projectile"))
        {
            if (collider.GetComponent<IDestroyable>().Destroy())
                Destroy(gameObject);
        }
        else if (!_stopDamage && (collider.CompareTag("Ennemy") || collider.CompareTag("Player") && _damagePlayer))
        {
            _stopDamage = true;
            collider.GetComponent<IDamageable>().Damage(_damage, 0);
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