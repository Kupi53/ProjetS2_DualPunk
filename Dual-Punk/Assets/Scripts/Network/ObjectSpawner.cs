using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using FishNet.Object;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

public class ObjectSpawner : NetworkBehaviour
{
    [ServerRpc (RequireOwnership = false)]
    public void SpawnObjectRpc(GameObject obj){
        Spawn(obj);
    }
}
