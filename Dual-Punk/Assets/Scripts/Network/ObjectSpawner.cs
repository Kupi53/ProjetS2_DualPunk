using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using FishNet.Connection;
using FishNet.Object;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class ObjectSpawner : NetworkBehaviour
{
    public static ObjectSpawner Instance;

    void Start()
    {
        Instance = this;
    }


    [ServerRpc (RequireOwnership = false)]
    public void SpawnObjectRpc(GameObject obj, Vector3 pos, Quaternion quaternion){
        GameObject instance = Instantiate(obj, pos, quaternion);
        Spawn(instance);
    }

    [ServerRpc (RequireOwnership = false)]
    public void SpawnObjectAndUpdateRpc(GameObject obj, Vector3 pos, Quaternion quaternion, NetworkConnection networkConnection, GameObject itemManager){
        obj.SetActive(true);
        UpdateHeldWeaponClientsRpc(networkConnection, obj, itemManager);
    }


    [TargetRpc]
    private void UpdateHeldWeaponClientsRpc(NetworkConnection networkConnection, GameObject instance, GameObject itemManager)
    {
        itemManager.GetComponent<ItemManager>().UpdateHeldWeapon(instance.GetComponent<WeaponScript>());
    }

    [ServerRpc (RequireOwnership = false)]
    public void SpawnWeapons(GameObject obj, Vector3 pos, Quaternion quaternion){

        obj.SetActive(true);
        obj.transform.position = pos;
    }
}
