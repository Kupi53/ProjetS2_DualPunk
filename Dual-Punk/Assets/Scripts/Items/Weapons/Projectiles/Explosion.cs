using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public void Explode(float damage, float explosionDistance, float explosionImpact)
    {
        GameObject[] ennemies = GameObject.FindGameObjectsWithTag("Ennemy");
        foreach (GameObject ennemy in ennemies)
        {
            Vector3 hitDirection = ennemy.transform.position - transform.position;
            if (hitDirection.magnitude <= explosionDistance)
            {
                ennemy.GetComponent<EnnemyState>().OnDamage(damage * (explosionDistance - hitDirection.magnitude) / explosionDistance);
            }
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Vector3 hitDirection = player.transform.position - transform.position;
            if (hitDirection.magnitude <= explosionDistance)
            {
                player.GetComponent<IImpact>().Impact(hitDirection, explosionImpact);
            }
        }
    }
}
