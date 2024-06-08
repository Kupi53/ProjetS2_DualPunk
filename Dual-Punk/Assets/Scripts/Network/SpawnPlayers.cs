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
    [SerializeField] private GameObject _objectSpawner;
    private Vector3 _PLAYER1SPAWNLOCATION;
    private Vector3 _PLAYER2SPAWNLOCATION;
    private GameObject playerObject;

    public override void OnStartClient(){
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
            _PLAYER1SPAWNLOCATION = GameObject.Find("Player1SpawnPoint").transform.position;
            GameObject floorWrapper = Instantiate(_floorWrapper);
            GameObject effectTileWrapper = Instantiate(_effectTilesNetworkWrapper);
            GameObject objectSpawner = Instantiate(_objectSpawner);
            Spawn(floorWrapper);
            Spawn(effectTileWrapper);
            Spawn(objectSpawner);
            playerObject = Instantiate(playerPrefabA, _PLAYER1SPAWNLOCATION, transform.rotation);
            GameManager.Instance.Player1 = playerObject;
        }
        else{
            _PLAYER2SPAWNLOCATION = GameObject.Find("Player2SpawnPoint").transform.position;
            playerObject = Instantiate(playerPrefabB, _PLAYER2SPAWNLOCATION, transform.rotation);
            GameManager.Instance.Player2 = playerObject;
        }
        base.Spawn(playerObject, connection);
        SetPlayer2Client(connection);
        if (GameManager.Instance.DebugMode)
        {
            GameObject.Find("Tutorial").GetComponentInChildren<EndOfTutorialTrigger>().StartGameRpc();
        }

    } 
    [TargetRpc]
    void SetPlayer2Client(NetworkConnection con)
    {
        GameManager.Instance.Player2 = GameObject.Find("Player2(Clone)");
    }
}
