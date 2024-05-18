using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChargedBulletScript : BulletScript
{
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
            collider.GetComponent<IDamageable>().Damage(_damage, 0);
            collider.GetComponent<IImpact>().Impact(_moveDirection, _impactForce);
            DestroyObject();
        }
        else if (collider.CompareTag("Wall"))
        {
            _collisionsAllowed--;
            _damage = (int)(_damage * 0.75f);

            if (_collisionsAllowed < 0)
            {
                DestroyObject();
            }
        }
    }
}