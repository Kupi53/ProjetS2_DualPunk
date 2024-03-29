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

public class ConnectionStarter : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button connectButton;
    [SerializeField] private Button debugButton;
    [SerializeField] private TMP_InputField codeField;

    private Tugboat _tugboat;
    private RelayManager _relayManager;

    private void Awake(){
        hostButton.onClick.AddListener(() => {
            LoadLobbyScene("host");
        });
        connectButton.onClick.AddListener(()=>{
            LoadLobbyScene("client");
        });
        debugButton.onClick.AddListener(() => {
            StartCoroutine(LoadGameDebug());
        });
    }
    void Start()
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
        if (TryGetComponent(out RelayManager RelayManager))
        {
            _relayManager = RelayManager;
        }
        else
        {
            Debug.LogError("Couldn't get tugboat!", this);
            return;
        }
    }
    void LoadLobbyScene(string type)
    {
        // lobby scene
        if (type=="host"){
            _relayManager.CreateRelay();
            _tugboat.StartConnection(true);
            _tugboat.StartConnection(false);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
        if (type == "client"){
            _relayManager.JoinRelay(codeField.text);
            _tugboat.StartConnection(false);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
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
        _tugboat.StartConnection(true);
        _tugboat.StartConnection(false);
        yield return new WaitForSeconds(0.5f);
        SceneLoadData sld = new SceneLoadData("Game");
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }
}