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

public class ConnectionStarter : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button connectButton;
    [SerializeField] private Button debugButton;
    [SerializeField] private TMP_InputField codeField;
    public string joinCode;
    private NetworkManager _networkManager;
    private RelayManager _relayManager;

    private void Awake(){
        hostButton.onClick.AddListener(() => {
            LoadLobbySceneAsync("host");
        });
        connectButton.onClick.AddListener(()=>{
            LoadLobbySceneAsync("client");
        });
        debugButton.onClick.AddListener(() => {
            StartCoroutine(LoadGameDebug());
        });
    }
    void Start()
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
        if (TryGetComponent(out RelayManager RelayManager))
        {
            _relayManager = RelayManager;
        }
        else
        {
            Debug.LogError("Couldn't get relay!", this);
            return;
        }
    }
    async Task LoadLobbySceneAsync(string type)
    {
        // lobby scene
        if (type=="host"){
            joinCode = await _relayManager.CreateRelayHost();
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
        if (type == "client"){
            _relayManager.JoinRelayClient(codeField.text);
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