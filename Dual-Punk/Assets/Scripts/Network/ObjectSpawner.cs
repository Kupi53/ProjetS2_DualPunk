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
    public GameObject LastSpawnedObject;

    void Start()
    {
        Instance = this;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsHost)
        {
            Instance = GameObject.FindWithTag("ObjectSpawner").GetComponent<ObjectSpawner>();
            Debug.Log(Instance.gameObject.name);
        }
    }

    [ServerRpc (RequireOwnership = false)]
    public void SpawnObjectRpc(GameObject obj, Vector3 pos, Quaternion quaternion)
    {
        GameObject instance = Instantiate(obj, pos, quaternion);
        Spawn(instance);
        if (!GameManager.Instance.InTutorial && !GameManager.Instance.InFinalFight)
        {
            instance.transform.SetParent(FloorNetworkWrapper.Instance.LocalFloorManager.CurrentRoom.gameObject.transform);
            GiveOwnerShipToAllClients(instance.GetComponent<NetworkObject>());
            ObjectParentToRoomClients(instance);
        }
        UpdateLastSpawnedObjectObservers(instance);
    }

    [ServerRpc (RequireOwnership = false)]
    public void SpawnObjectFromIdRpc(string Id, Vector3 pos, Quaternion quaternion)
    {
        if (Id == "null")
        {
            return;
        }
        else
        {
            GameObject prefab = ItemIds.Instance.IdTable[Id];
            GameObject instance = Instantiate(prefab, pos, quaternion);
            Spawn(instance);
            if (!GameManager.Instance.InTutorial && !GameManager.Instance.InFinalFight)
            {
                instance.transform.SetParent(FloorNetworkWrapper.Instance.LocalFloorManager.CurrentRoom.gameObject.transform);
                GiveOwnerShipToAllClients(instance.GetComponent<NetworkObject>());
                ObjectParentToRoomClients(instance);
            }
            UpdateLastSpawnedObjectObservers(instance);
        }
    }

    [ServerRpc (RequireOwnership = false)]
    public void SpawnObjectFromIdAndEquipToTargetRpc(string Id, Vector3 pos, Quaternion quaternion, NetworkConnection con)
    {
        if (Id == "null")
        {
            return;
        }
        else
        {
            GameObject prefab = ItemIds.Instance.IdTable[Id];
            GameObject instance = Instantiate(prefab, pos, quaternion);
            Spawn(instance);
            if (!GameManager.Instance.InTutorial && !GameManager.Instance.InFinalFight)
            {
                instance.transform.SetParent(FloorNetworkWrapper.Instance.LocalFloorManager.CurrentRoom.gameObject.transform);
                GiveOwnerShipToAllClients(instance.GetComponent<NetworkObject>());
                ObjectParentToRoomClients(instance);
            }
            UpdateLastSpawnedObjectObservers(instance);
            if (con.IsHost)
            {
                EquipToTargetRpc(con, instance);
            }
        }
    }

    [TargetRpc]
    public void EquipToTargetRpc(NetworkConnection con, GameObject obj)
    {
        GameManager.Instance.LocalPlayer.GetComponent<ItemManager>().Picking(obj);
    }

    [ServerRpc (RequireOwnership = false)]
    public void SpawnObjectAndUpdateRpc(GameObject obj, Vector3 pos, Quaternion quaternion, NetworkConnection networkConnection, GameObject itemManager)
    {
        UpdateHeldWeaponClientsRpc(networkConnection, obj, itemManager);
    }

    [TargetRpc]
    private void UpdateHeldWeaponClientsRpc(NetworkConnection networkConnection, GameObject obj, GameObject itemManager)
    {
        itemManager.GetComponent<ItemManager>().UpdateHeldWeapon(obj.GetComponent<WeaponScript>());
    }
    #nullable enable
    [ServerRpc (RequireOwnership = false)]
    public void RemoveOwnershipFromNonOwnersRpc(GameObject obj, NetworkConnection? owner)
    {
        if (obj.GetComponent<NetworkObject>() == null)
        {
            Debug.Log("je suis une merde");
            return;
        }
        Debug.Log(obj);
        obj.GetComponent<NetworkObject>().RemoveOwnership();
        if (owner is not null)
        {
            GiveOwnershipRPC(obj.GetComponent<NetworkObject>(), owner);
        }
    }
    #nullable disable

    [ServerRpc (RequireOwnership = false)]
    public void SpawnWeapons(GameObject obj, Vector3 pos, Quaternion quaternion){

        obj.transform.position = pos;
        obj.SetActive(true);
    }
    
    // if parent is null, offset will be the new position of the object
    [ServerRpc (RequireOwnership = false)]
    public void ObjectParentToGameObjectRpc(GameObject obj, GameObject parent, Vector3 offset)
    {
        GiveOwnerShipToAllClients(obj.GetComponent<NetworkObject>());
        ObjectParentToGameObjectClients(obj, parent, offset);
    }
    
    [ServerRpc (RequireOwnership = false)]
    public void ObjectParentToRoomRpc(GameObject obj)
    {
        GiveOwnerShipToAllClients(obj.GetComponent<NetworkObject>());
        ObjectParentToRoomClients(obj);
    }
    [ServerRpc (RequireOwnership = false)]
    public void RemoveParentRpc(GameObject obj)
    {
        RemoveParentClients(obj);
    }

    [ObserversRpc]
    public void RemoveParentClients(GameObject obj)
    {
        obj.transform.SetParent(null);
    }
    [ObserversRpc]
    public void ObjectParentToRoomClients(GameObject obj)
    {
        obj.transform.SetParent(FloorNetworkWrapper.Instance.LocalFloorManager.CurrentRoom.gameObject.transform);
    }

    [ObserversRpc]
    public void ObjectParentToGameObjectClients(GameObject obj, GameObject parent, Vector3 offset)
    {
        if (parent == null)
        {
            obj.transform.SetParent(null);
        }
        else
        {
            obj.transform.SetParent(parent.transform);
        }
        obj.transform.localPosition = offset;
    } 
    
    [ObserversRpc]
    void GiveOwnerShipToAllClients(NetworkObject obj)
    {
        GiveOwnershipRPC(obj, ClientManager.Connection);
    }
    [ServerRpc (RequireOwnership = false)]
    void GiveOwnershipRPC(NetworkObject networkObject, NetworkConnection networkConnection)
    {
        networkObject.GiveOwnership(networkConnection);
    }

    [ObserversRpc]
    void UpdateLastSpawnedObjectObservers(GameObject obj)
    {
        LastSpawnedObject = obj;
    }

    [ServerRpc (RequireOwnership = false)]
    public void ImplantRendererSetEnabledRPC(GameObject implant, bool enabled)
    {
        ImplantRendererSetEnabledObs(implant, enabled);
    }

    [ObserversRpc]
    private void ImplantRendererSetEnabledObs(GameObject implant, bool enabled)
    {
        implant.GetComponent<SpriteRenderer>().enabled = enabled;
    }

    [ServerRpc (RequireOwnership = false)]
    public void ImplantSetIsEquippedRPC(GameObject implant, bool equipped)
    {
        ImplantSetIsEquippedObs(implant, equipped);
    }

    [ObserversRpc]
    private void ImplantSetIsEquippedObs(GameObject implant, bool equipped)
    {
        implant.GetComponent<ImplantScript>().IsEquipped = equipped;
    }

}
