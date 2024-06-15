using FishNet.Demo.AdditiveScenes;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class Explosion : NetworkBehaviour
{
    public void Explode(int damage, float explosionRadius, float explosionImpact, bool damagePlayer, bool warriorLuck)
    {
        DamageVictims("Ennemy", damage, explosionRadius, explosionImpact, damagePlayer, warriorLuck);
        DamageVictims("Player", damage, explosionRadius, explosionImpact, !damagePlayer, warriorLuck);
        DamageVictims("Projectile", damage, explosionRadius, explosionImpact, false, false);

        Destroy(gameObject, 2);
    }


    private void DamageVictims(string tagDeLaVictime, int damage, float explosionRadius, float explosionImpact, bool dealDamage, bool warriorLuck)
    {
        GameObject[] victimes = GameObject.FindGameObjectsWithTag(tagDeLaVictime);
        foreach (GameObject grosseVictime in victimes)
        {
            Vector3 hitDirection = grosseVictime.transform.position - transform.position;
            float distance = hitDirection.magnitude;

            if (grosseVictime.layer == 9)
            {
                if (distance <= explosionRadius / 2)
                    grosseVictime.GetComponent<IDestroyable>().DestroyObject();
                continue;
            }

            if (distance <= explosionImpact * 6)
            {
                if (tagDeLaVictime == "Player")
                    grosseVictime.GetComponent<PlayerState>().CameraController.ShakeCamera(explosionImpact * (1 - distance / (explosionRadius * 6)), 0.25f);
                
                if (tagDeLaVictime == "Ennemy")
                    grosseVictime.GetComponent<NPCState>().TargetPoint = transform.position;
            }
            
            if (distance <= explosionRadius)
            {
                grosseVictime.GetComponent<IImpact>().Impact(hitDirection, explosionImpact * (1 - distance / explosionRadius));

                if (dealDamage)
                    grosseVictime.GetComponent<IDamageable>().Damage((int)(damage * (explosionRadius - distance) / explosionRadius), 0.25f, warriorLuck, 0f);
            }
        }
    }


    [ObserversRpc]
    private void ShakeCamera(PlayerState playerState, float intensity)
    {
        playerState.CameraController.ShakeCamera(intensity, 0.25f);
    }

    [ObserversRpc]
    private void HitPlayer(GameObject player, Vector3 direction, float intensity, int damage, bool crit)
    {
        player.GetComponent<IImpact>().Impact(direction, intensity);
        if (damage > 0)
            player.GetComponent<IDamageable>().Damage(damage, 0.25f, crit, 0f);
    }
}
