using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Explosion : NetworkBehaviour
{
    public void Explode(int damage, float explosionRadius, float explosionImpact)
    {
        ExplodeRPC(damage, explosionRadius, explosionImpact);
    }


    [ServerRpc(RequireOwnership = false)]
    private void ExplodeRPC(int damage, float explosionRadius, float explosionImpact)
    {
        GameObject[] ennemies = GameObject.FindGameObjectsWithTag("Ennemy");
        foreach (GameObject ennemy in ennemies)
        {
            Vector3 hitDirection = ennemy.transform.position - transform.position;
            if (hitDirection.magnitude <= explosionRadius)
            {
                ennemy.GetComponent<EnemyHealthManager>().Damage((int)(damage * (explosionRadius - hitDirection.magnitude) / explosionRadius), 0);
            }
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(players.Length);
        foreach (GameObject player in players)
        {
            Vector3 hitDirection = player.transform.position - transform.position;
            float distance = hitDirection.magnitude;

            if (distance <= explosionRadius * 10)
            {
                float multiplier = 1 - distance / explosionRadius;
                player.GetComponent<PlayerState>().CameraController.ShakeCamera(explosionImpact * multiplier, 0.5f);

                if (distance <= explosionRadius)
                {
                    player.GetComponent<IImpact>().Impact(hitDirection, explosionImpact * multiplier);
                }
            }
        }

        Destroy(gameObject, 2);
    }
}
