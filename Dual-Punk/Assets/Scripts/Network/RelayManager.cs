using System.Threading.Tasks;
using FishNet.Managing;
using FishNet.Managing.Client;
using FishNet.Object;
using FishNet.Transporting;
using FishNet.Transporting.UTP;
using TMPro;
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

        networkManager.ClientManager.OnClientConnectionState += (args) =>
        {
            ClientTimeout(args);
        };
    }

    public async Task<string> CreateRelayHost(){
        // 1 max connection car host pas inclu
        try{
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            string joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            _fishyUTP.SetRelayServerData(new RelayServerData(allocation, "dtls"));
            _networkManager.ServerManager.StartConnection();
            _networkManager.ClientManager.StartConnection();
            return joinCode ;
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
            SpawnNetworkErrorMessage("Could not find/connect to the server.");
        }
    }

    public static void SpawnNetworkErrorMessage(string errorMessage){
        //  Assets/Resources/NetworkError
        GameObject errorUI = (GameObject)Instantiate(Resources.Load("NetworkError"));
        errorUI.GetComponentInChildren<TMP_Text>().text += errorMessage;
        errorUI.transform.SetParent(GameObject.Find("Canvas").transform, false);
    }

    public static void ClientTimeout(ClientConnectionStateArgs args){
        if (args.ConnectionState == LocalConnectionState.Stopping){
            Debug.Log("disconnected");
            LobbyMenu.LoadMenu();
            SpawnNetworkErrorMessage("Client timed out.");
        }
    }
}
