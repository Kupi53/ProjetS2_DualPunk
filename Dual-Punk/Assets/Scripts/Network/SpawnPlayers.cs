using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Connection;
using UnityEngine;
using FishNet.Managing;

public class SpawnPlayers : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefabA; //add prefab in inspector
    [SerializeField] private GameObject playerPrefabB; //add prefab in inspector
    [SerializeField] private GameObject _floorManager;
    private GameObject playerObject;
    private GameObject networkManager;

    public override void OnStartClient(){
        networkManager = GameObject.FindWithTag("NetworkManager");
        if (IsServer){
            SpawnServerRPC(ClientManager.Connection, true);
        }
        else{
            SpawnServerRPC(ClientManager.Connection, false);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void SpawnServerRPC(NetworkConnection connection, bool host){
        if (host){
            playerObject = Instantiate(playerPrefabA, new Vector3(0,0,0), transform.rotation);
            GameManager.Instance.Player1 = playerObject;
            GameObject floorManager = Instantiate(_floorManager);
            Spawn(floorManager);
        }
        else{
            playerObject = Instantiate(playerPrefabB, new Vector3(0,0,0), transform.rotation);
            GameManager.Instance.Player2 = playerObject;
        }
        base.Spawn(playerObject, connection);
    } 
}
