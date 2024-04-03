using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FishNet;
using FishNet.Connection;
using FishNet.Broadcast;
using FishNet.Object;
using TMPro;
using Unity.VisualScripting;
using FishNet.Transporting.Tugboat;
using FishNet.Managing.Scened;
using Unity.Services.Relay;
using FishNet.Transporting.UTP;
using FishNet.Managing;
using System.Threading.Tasks;
using Unity.VisualScripting.Dependencies.Sqlite;

public class ConnectionStarter : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button connectButton;
    [SerializeField] private Button debugButton;
    [SerializeField] private TMP_InputField codeField;
    public string joinCode;
    private NetworkManager _networkManager;
    private RelayManager _relayManager;

    void Start()
    {
        hostButton.onClick.AddListener(() => {
            _ = LoadLobbySceneAsync("host");
        });
        connectButton.onClick.AddListener(()=>{
            _ = LoadLobbySceneAsync("client");
        });
        debugButton.onClick.AddListener(() => {
            StartCoroutine(LoadGameDebug());
        });
        _networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        _relayManager = GameObject.Find("NetworkManager").GetComponent<RelayManager>();
        GameObject _servercm = GameObject.Find("ServerConnectionsManager");
        if (_servercm is not null){
            Destroy(_servercm);
        }
        Debug.Log("sdfsd");
    }
    async Task LoadLobbySceneAsync(string type)
    {
        // lobby scene
        if (type=="host"){
            await _relayManager.CreateRelayHost();
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
        if (type == "client"){
            if (codeField.text != ""){
                _relayManager.JoinRelayClient(codeField.text);
            }
            else{
                StartCoroutine(RelayManager.SpawnNetworkErrorMessage("Join code cannot be empty"));
            }
        }
    }

    public void OnIpInputChanged(){
        if (codeField.text == ""){
        }
        else{
            connectButton.interactable = true;
            connectButton.GetComponentInChildren<TMP_Text>().color = new Color32(0, 185, 255, 255);

        }
    }

    IEnumerator LoadGameDebug()
    {
        _networkManager.ServerManager.StartConnection();
        _networkManager.ClientManager.StartConnection();
        yield return new WaitForSeconds(0.5f);
        SceneLoadData sld = new SceneLoadData("Game");
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }
}