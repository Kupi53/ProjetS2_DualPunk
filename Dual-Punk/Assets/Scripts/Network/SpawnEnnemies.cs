using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Networking;
using System.Security.Cryptography;

public class SpawnEnnemies : NetworkBehaviour
{
    [SerializeField] private GameObject randomEnnemy; //add prefab in inspector
    private NetworkObject spawnedEnnemy;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;  
        GameObject bot = Instantiate(randomEnnemy, new Vector3(-0.51f,-4.29f,0), Quaternion.identity);
        spawnedEnnemy = bot.GetComponent<NetworkObject>();
        spawnedEnnemy.Spawn();
        /*GameObject bot2 = Instantiate(randomEnnemy, new Vector3(-0.1f, -5f, 0), Quaternion.identity);
        spawnedEnnemy = bot2.GetComponent<NetworkObject>();
        spawnedEnnemy.Spawn();
        GameObject bot3 = Instantiate(randomEnnemy, new Vector3(-0.3f, -3f, 0), Quaternion.identity);
        spawnedEnnemy = bot3.GetComponent<NetworkObject>();
        spawnedEnnemy.Spawn();
        GameObject bot4 = Instantiate(randomEnnemy, new Vector3(-1f, -5f, 0), Quaternion.identity);
        spawnedEnnemy = bot4.GetComponent<NetworkObject>();
        spawnedEnnemy.Spawn();*/
    }
}
