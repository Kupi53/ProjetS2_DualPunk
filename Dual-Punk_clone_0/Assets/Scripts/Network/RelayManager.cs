using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using FishNet.Managing;
using FishNet.Transporting.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    private FishyUnityTransport _fishyUTP;
    private NetworkManager _networkManager;

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


        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);            
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRelayHost(){
        // 1 max connection car host pas inclu
        try{
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            string joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
            _fishyUTP.SetRelayServerData(new RelayServerData(allocation, "dtls"));
            _networkManager.ServerManager.StartConnection();
            _networkManager.ClientManager.StartConnection();
        }
        catch (RelayServiceException e){
            Debug.Log(e);
        }
    }

    public async void JoinRelayClient(string joinCode){
        try{
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            _fishyUTP.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            _networkManager.ClientManager.StartConnection();
        }
        catch (RelayServiceException e){
            Debug.Log(e);
        }
    }
}
