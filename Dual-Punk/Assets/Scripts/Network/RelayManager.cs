using System.Collections;
using System.Collections.Generic;
using System.Data;
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

public class RelayManager : MonoBehaviour
{
    private FishyUnityTransport _fishyUTP;
    private NetworkManager _networkManager;
    public string joinCode;
    public bool clientRequestedDisconnect;

    private async void Start()
    {
        if (TryGetComponent(out NetworkManager networkManager))
        {
            _networkManager = networkManager;
        }
        else
        {
            Debug.LogError("Couldn't get networkmanager!", this);
            return;
        }
        if (TryGetComponent(out FishyUnityTransport fishyUTP))
        {
            _fishyUTP = fishyUTP;
        }
        else
        {
            Debug.LogError("Couldn't get UTP!", this);
            return;
        }
        if (UnityServices.State != ServicesInitializationState.Initialized){
            await UnityServices.InitializeAsync();
        }

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);            
        };

        if (!AuthenticationService.Instance.IsSignedIn){
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        _networkManager.ClientManager.OnClientConnectionState += (ClientConnectionStateArgs args) => {
            ClientTimeout(args);
        };
        

    }

    public async Task<string> CreateRelayHost(){
        // 1 max connection car host pas inclu
        try{
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            _fishyUTP.SetRelayServerData(new RelayServerData(allocation, "dtls"));
            _networkManager.ServerManager.StartConnection();
            _networkManager.ClientManager.StartConnection();
            return joinCode;
        }
        catch (RelayServiceException e){

            Debug.Log(e);
            SpawnNetworkErrorMessage("Could not create the server.");
            throw new RelayServiceException(e);
        }
    }

    public async void JoinRelayClient(string joinCode){
        try{
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            _fishyUTP.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            _networkManager.ClientManager.StartConnection();
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
        catch (RelayServiceException e){
            Debug.Log(e);
            StartCoroutine(SpawnNetworkErrorMessage("Could not find/connect to the server."));
        }
    }

    public static IEnumerator SpawnNetworkErrorMessage(string errorMessage){
        //  Assets/Resources/NetworkError
        while(SceneManager.GetActiveScene().name != "Menu"){
            yield return null;
        }
        GameObject errorUI = (GameObject)Instantiate(Resources.Load("NetworkError"));
        errorUI.GetComponentInChildren<TMP_Text>().text += errorMessage;
        errorUI.transform.SetParent(GameObject.Find("Canvas").transform, false);
    }

    private void ClientTimeout(ClientConnectionStateArgs args){
        if (_networkManager.IsServer) return;
        if (args.ConnectionState == LocalConnectionState.Stopped){
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
            if (clientRequestedDisconnect){
                clientRequestedDisconnect = false;
            }
            else{
                StartCoroutine(SpawnNetworkErrorMessage("Client timed out."));
            }
        }
    }
}