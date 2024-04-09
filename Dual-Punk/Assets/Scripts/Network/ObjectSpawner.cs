using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using FishNet.Object;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

public class ObjectSpawner : NetworkBehaviour
{

    public GameObject SpawnObject(GameObject obj, UnityEngine.Vector3 transform, Quaternion quat)
    {
        GameObject instance = Instantiate(obj, transform, quat);
        instance.SetActive(true);
        Debug.Log(instance.GetComponent<NetworkObject>());
        SpawnObjectRpc(instance.GetComponent<NetworkObject>());
        return instance;        
    }


    [ServerRpc (RequireOwnership = false)]
    public void SpawnObjectRpc(NetworkObject obj){
        Spawn(obj);
    }
}
