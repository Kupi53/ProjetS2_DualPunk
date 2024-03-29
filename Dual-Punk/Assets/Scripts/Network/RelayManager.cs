using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using FishNet.Managing;
using FishNet.Transporting.Tugboat;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    private Tugboat _tugboat;

    private async void Start()
    {
        if (TryGetComponent(out Tugboat tugboat))
        {
            _tugboat = tugboat;
        }
        else
        {
            Debug.LogError("Couldn't get tugboat!", this);
            return;
        }


        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);            
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRelay(){
        // 1 max connection car host pas inclu
        try{
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            string joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
            _tugboat.SetServerBindAddress(allocation.RelayServer.IpV4, FishNet.Transporting.IPAddressType.IPv4);
            _tugboat.SetClientAddress(allocation.RelayServer.IpV4);
            _tugboat.SetPort((ushort)allocation.RelayServer.Port);
            _tugboat.SetMaximumClients(2);
        }
        catch (RelayServiceException e){
            Debug.Log(e);
        }
    }

    public async void JoinRelay(string joinCode){
        try{
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            _tugboat.SetClientAddress(joinAllocation.RelayServer.IpV4);
            _tugboat.SetPort((ushort)joinAllocation.RelayServer.Port);
        }
        catch (RelayServiceException e){
            Debug.Log(e);
        }
    }
}
