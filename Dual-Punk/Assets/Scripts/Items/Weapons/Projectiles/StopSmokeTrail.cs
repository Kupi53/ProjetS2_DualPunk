using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopSmokeTrail : NetworkBehaviour
{
    [ObserversRpc]
    public void StopParticles()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<ParticleSystem>().Stop();
        }
        Destroy(gameObject, 10);
    }
}
