using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Networking;
using System.Security.Cryptography;

public class SpawnEnnemies : NetworkBehaviour
{
    [SerializeField] private GameObject randomEnnemy; //add prefab in inspector

    public override void OnNetworkSpawn()
    {
        GameObject bot = Instantiate(randomEnnemy, new Vector3(0,0,0), Quaternion.identity).gameObject;
        bot.GetComponent<NetworkObject>().Spawn(NetworkManager.Singleton);
    }
}
