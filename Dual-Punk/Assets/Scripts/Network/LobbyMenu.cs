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
using System.Runtime.InteropServices.WindowsRuntime;

public class LobbyMenu : NetworkBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private GameObject waiting;
    [SerializeField] private GameObject ready;
    private GameObject networkManager;
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
    void LoadMenu()
    {
        Destroy(fishnetDDOL);
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