using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Client;
using FishNet.Managing.Server;
using FishNet.Object;
using FishNet.Transporting;
using FishNet.Transporting.UTP;
using Pathfinding;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerConnectionsManager : NetworkBehaviour
{
    public static ServerConnectionsManager Instance;

    private NetworkManager networkManager;
    private void Awake()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    void Start(){
        Instance = this;
    }


    public void CloseConnection(){
        DisconnectClientsRPC();
        networkManager.ServerManager.StopConnection(true);
    }

    [ObserversRpc]
    void DisconnectClientsRPC(){
        networkManager.ClientManager.StopConnection();
    } 

}