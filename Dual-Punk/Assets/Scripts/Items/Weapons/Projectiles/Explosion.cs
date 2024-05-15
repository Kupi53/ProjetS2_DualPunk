using FishNet.Demo.AdditiveScenes;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Explosion : NetworkBehaviour
{
    public void Explode(int damage, float explosionRadius, float explosionImpact, bool damagePlayer)
    {
        DamageVictims("Ennemy", damage, explosionRadius, explosionImpact, !damagePlayer);
        DamageVictims("Player", damage, explosionRadius, explosionImpact, damagePlayer);
        DamageVictims("Projectile", damage, explosionRadius, explosionImpact, false);

        Destroy(gameObject, 2);
    }


    private void DamageVictims(string tagDeLaVictime, int damage, float explosionRadius, float explosionImpact, bool dealDamage)
    {
        GameObject[] victimes = GameObject.FindGameObjectsWithTag(tagDeLaVictime);
        foreach (GameObject grosseVictime in victimes)
        {
            Vector3 hitDirection = grosseVictime.transform.position - transform.position;
            float distance = hitDirection.magnitude;

            if (tagDeLaVictime == "Player" && distance <= explosionRadius * 6)
            {
                ShakeCamera(grosseVictime.GetComponent<PlayerState>(), explosionImpact * (1 - distance / (explosionRadius * 6)));
            }
            else if (tagDeLaVictime == "Projectile" && distance < explosionRadius * 0.2f)
            {
                grosseVictime.GetComponent<IDestroyable>().DestroyObject();
                continue;
            }

            if (distance <= explosionRadius)
            {
                grosseVictime.GetComponent<IImpact>().Impact(hitDirection, explosionImpact * (1 - distance / explosionRadius));

                if (dealDamage)
                    grosseVictime.GetComponent<IDamageable>().Damage((int)(damage * (explosionRadius - distance) / explosionRadius), 0.25f);
            }
        }
    }


    [ObserversRpc]
    private void ShakeCamera(PlayerState playerState, float intensity)
    {
        if (playerState.CameraController == null) return;

        playerState.CameraController.ShakeCamera(intensity, 0.25f);
    }
}
