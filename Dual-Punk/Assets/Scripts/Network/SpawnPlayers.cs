using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnPlayers : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefabA; //add prefab in inspector
    [SerializeField] private GameObject playerPrefabB; //add prefab in inspector

    public override void OnNetworkSpawn(){
        if (IsHost){
            GameObject player = Instantiate(playerPrefabA, new Vector3(0,0,0), transform.rotation);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);
        }
        else{
            SpawnServerRPC(NetworkManager.Singleton.LocalClientId);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void SpawnServerRPC(ulong clientId){
        GameObject player = Instantiate(playerPrefabB, new Vector3(0,0,0), transform.rotation);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    } 

}
