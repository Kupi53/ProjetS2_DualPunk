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
    [SerializeField] private GameObject _floorWrapper;
    [SerializeField] private GameObject _effectTilesNetworkWrapper;
    private GameObject playerObject;
    private GameObject networkManager;

    public override void OnStartClient(){
        networkManager = GameObject.FindWithTag("NetworkManager");
        if (IsServer){
            SpawnServerRPC(ClientManager.Connection, true);
        }
        else{
            SpawnServerRPC(ClientManager.Connection, false);
            GameManager.Instance.Player1 = GameObject.Find("Player1(Clone)");
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void SpawnServerRPC(NetworkConnection connection, bool host){
        if (host){
            playerObject = Instantiate(playerPrefabA, new Vector3(0,0,0), transform.rotation);
            GameManager.Instance.Player1 = playerObject;
            GameObject floorWrapper = Instantiate(_floorWrapper);
            Spawn(floorWrapper);
            GameObject effectTileWrapper = Instantiate(_effectTilesNetworkWrapper);
            Spawn(effectTileWrapper);
        }
        else{
            playerObject = Instantiate(playerPrefabB, new Vector3(0,0,0), transform.rotation);
            GameManager.Instance.Player2 = playerObject;
        }
        base.Spawn(playerObject, connection);
        SetPlayer2Client(connection);

    } 
    [TargetRpc]
    void SetPlayer2Client(NetworkConnection con)
    {
        GameManager.Instance.Player2 = GameObject.Find("Player2(Clone)");
    }
}
