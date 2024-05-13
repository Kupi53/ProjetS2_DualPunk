using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Explosion : NetworkBehaviour
{
    public void Explode(int damage, float explosionRadius, float explosionImpact, bool damagePlayer)
    {
        GameObject[] ennemies = GameObject.FindGameObjectsWithTag("Ennemy");
        foreach (GameObject ennemy in ennemies)
        {
            Vector3 hitDirection = ennemy.transform.position - transform.position;
            float distance = hitDirection.magnitude;

            if (distance <= explosionRadius)
            {
                
                if (!damagePlayer)
                    ennemy.GetComponent<EnemyHealthManager>().Damage((int)(damage * (explosionRadius - distance) / explosionRadius), 0);
            }
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(players.Length);

        foreach (GameObject player in players)
        {
            Vector3 hitDirection = player.transform.position - transform.position;
            float distance = hitDirection.magnitude;

            if (distance <= explosionRadius * 6)
            {
                player.GetComponent<PlayerState>().CameraController.ShakeCamera(explosionImpact * (1 - distance / (explosionRadius * 6)), 0.25f);

                if (distance <= explosionRadius)
                {
                    player.GetComponent<IImpact>().Impact(hitDirection, explosionImpact * (1 - distance / explosionRadius));
                    if 
                    
                }
            }
        }

        Destroy(gameObject, 2);
    }
}
