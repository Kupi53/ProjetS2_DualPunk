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
        GameObject bot = Instantiate(randomEnnemy, new Vector3(-0.51f,-4.29f,0), Quaternion.identity);
        spawnedEnnemy = bot.GetComponent<NetworkObject>();
        spawnedEnnemy.Spawn();
      }
}
