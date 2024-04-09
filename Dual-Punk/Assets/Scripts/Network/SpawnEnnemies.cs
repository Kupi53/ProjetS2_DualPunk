using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Networking;
using System.Security.Cryptography;
using FishNet.Object;

public class SpawnEnnemies : NetworkBehaviour
{
    [SerializeField] private GameObject randomEnnemy; //add prefab in inspector
    private NetworkObject spawnedEnnemy;
    
    public override void OnStartServer()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject bot = Instantiate(randomEnnemy, new Vector3(-0.50f * i, -1 * i, 0), Quaternion.identity);
            Spawn(bot);
        }
    }
    
}
