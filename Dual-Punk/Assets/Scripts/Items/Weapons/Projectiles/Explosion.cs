using FishNet.Connection;
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
        DamageVictims("Ennemy", damage, explosionRadius, explosionImpact, !damagePlayer, warriorLuck);
        DamageVictims("Player", damage, explosionRadius, explosionImpact, damagePlayer, warriorLuck);
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
                    ShakeCamera(grosseVictime.GetComponent<NetworkObject>().Owner, grosseVictime.GetComponent<PlayerState>(), explosionImpact * (1 - distance / (explosionRadius * 6)));
                
                if (tagDeLaVictime == "Ennemy")
                    grosseVictime.GetComponent<NPCState>().TargetPoint = transform.position;
            }
            
            if (distance <= explosionRadius)
            {
                ImpactTarget(grosseVictime.GetComponent<NetworkObject>().Owner, grosseVictime, hitDirection, explosionImpact * (1 - distance / explosionRadius));

                if (dealDamage)
                    grosseVictime.GetComponent<IDamageable>().Damage((int)(damage * (explosionRadius - distance) / explosionRadius), 0.25f, warriorLuck, 0f);
            }
        }
    }


    [TargetRpc]
    private void ShakeCamera(NetworkConnection con, PlayerState playerState, float intensity)
    {
        if (playerState == null) return;

        playerState.CameraController.ShakeCamera(intensity, 0.25f);
    }

    [TargetRpc]
    private void ImpactTarget(NetworkConnection con, GameObject target, Vector3 hitDir, float intensity)
    {
        target.GetComponent<IImpact>().Impact(hitDir, intensity);
    }
}
