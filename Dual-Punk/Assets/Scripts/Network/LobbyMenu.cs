using TMPro;
using FishNet;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FishNet.Object;
using FishNet.Managing.Scened;
using Unity.Services.Authentication;
using FishNet.Managing;

public class LobbyMenu : NetworkBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private GameObject waiting;
    [SerializeField] private GameObject ready;
    [SerializeField] private GameObject joinCodeText;
    [SerializeField] private GameObject serverConnectionsManagerPrefab;
    private GameObject networkManager;
    private RelayManager relayManager;
    private GameObject fishnetDDOL;
    private ServerConnectionsManager serverConnectionsManager;

    // Start is called before the first frame update
    void Awake()
    {
        cancelButton.onClick.AddListener(() => {
            LoadMenu();
        });
        playButton.onClick.AddListener(()=>{
            LoadGame();
        });
    }

    void Start(){
        networkManager = GameObject.FindWithTag("NetworkManager");
        fishnetDDOL = GameObject.Find("FirstGearGames DDOL");
        relayManager = networkManager.GetComponent<RelayManager>();
        if (networkManager.GetComponent<NetworkManager>().IsServer){
            serverConnectionsManager = Instantiate(serverConnectionsManagerPrefab).GetComponent<ServerConnectionsManager>();
        }
    }

    void Update(){
        if (!InstanceFinder.IsServer) return;
        ChangePlayButton();
    }

    public override void OnStartServer(){
        base.OnStartServer();
        Spawn(serverConnectionsManager.gameObject);
        cancelButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.34541f, -40, 0);
        playButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.34541f, 5, 0);
        relayManager = networkManager.GetComponent<RelayManager>();
        joinCodeText.GetComponent<TMP_Text>().text += relayManager.joinCode;
        joinCodeText.SetActive(true);
    }


    [Server]
    private void ChangePlayButton(){
        if (ServerManager.Clients.Count == 2){
            SetReadyTextClientRpc(true);
            playButton.interactable = true;
            playButton.GetComponentInChildren<TMP_Text>().color = new Color32(0, 185, 255, 255);
        }
        else{
            SetReadyTextClientRpc(false);
            playButton.interactable = false;
            playButton.GetComponentInChildren<TMP_Text>().color = new Color32(0, 185, 255, 100);
        }
    }
    public void LoadMenu()
    {
        if (networkManager.GetComponent<NetworkManager>().IsServer){
            serverConnectionsManager.CloseConnection();
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
        else{
            relayManager.clientRequestedDisconnect = true;
            networkManager.GetComponent<NetworkManager>().ClientManager.StopConnection();
        }
    }
    void LoadGame()
    {
        SceneLoadData sld = new SceneLoadData("Game");
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }

    [ObserversRpc]
    void SetReadyTextClientRpc(bool readyB){
        if (readyB){
            waiting.SetActive(false);
            ready.SetActive(true);
        }
        else{
            ready.SetActive(false);
            waiting.SetActive(true);
        }
    }
}