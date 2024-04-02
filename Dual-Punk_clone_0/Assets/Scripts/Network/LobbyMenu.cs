using TMPro;
using FishNet;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FishNet.Object;
using FishNet.Managing.Scened;
using Unity.Services.Authentication;

public class LobbyMenu : NetworkBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private GameObject waiting;
    [SerializeField] private GameObject ready;
    [SerializeField] private GameObject joinCodeText;
    private GameObject networkManager;
    private ConnectionStarter connectionStarter;
    private GameObject fishnetDDOL;

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
    }

    void Update(){
        if (!InstanceFinder.IsServer) return;
        ChangePlayButton();
    }

    public override void OnStartServer(){
        base.OnStartServer();
        cancelButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.34541f, -40, 0);
        playButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.34541f, 5, 0);
        connectionStarter = networkManager.GetComponent<ConnectionStarter>();
        joinCodeText.GetComponent<TMP_Text>().text += connectionStarter.joinCode;
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
    public static void LoadMenu()
    {
        GameObject _networkManager = GameObject.FindWithTag("NetworkManager");
        GameObject _fishnetDDOL = GameObject.Find("FirstGearGames DDOL");
        Destroy(_fishnetDDOL);
        Destroy(_networkManager);
        AuthenticationService.Instance.SignOut();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
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