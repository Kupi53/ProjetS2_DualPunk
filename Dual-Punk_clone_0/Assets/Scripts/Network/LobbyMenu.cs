using System.Collections;
using System.Collections.Generic;
using TMPro;
using FishNet;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FishNet.Object;
using FishNet.Managing;
using FishNet.Managing.Scened;

public class LobbyMenu : NetworkBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private GameObject waiting;
    [SerializeField] private GameObject ready;
    private int connectedCount = 0;
    private GameObject networkManager;
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
        RpcClientConnected();
    }
    void Update(){
        ChangePlayButton();
    }

    [Server]
    private void ChangePlayButton(){
        if (connectedCount == 2){
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

    [ServerRpc (RequireOwnership = false)]
    private void RpcClientConnected(){
        connectedCount += 1;
    }

    void LoadMenu()
    {
        Destroy(networkManager);
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