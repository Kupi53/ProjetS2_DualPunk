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

public class ConnectionStarter : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button connectButton;
    [SerializeField] private Button debugButton;
    [SerializeField] private TMP_InputField ipField;

    private Tugboat _tugboat;

    private void Awake(){
        hostButton.onClick.AddListener(() => {
            LoadLobbyScene("host");
        });
        connectButton.onClick.AddListener(()=>{
            LoadLobbyScene("client");
        });
        debugButton.onClick.AddListener(() => {
            LoadGameDebug();
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
    }
    void LoadLobbyScene(string type)
    {
        // lobby scene
        if (type=="host"){
            _tugboat.StartConnection(true);
            _tugboat.StartConnection(false);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
        if (type == "client"){
            _tugboat.SetClientAddress(ipField.text);
            _tugboat.StartConnection(false);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
    }

    public void OnIpInputChanged(){
        if (ipField.text == ""){
        }
        else{
            connectButton.interactable = true;
            connectButton.GetComponentInChildren<TMP_Text>().color = new Color32(0, 185, 255, 255);

        }
    }

    void LoadGameDebug()
    {
        _tugboat.StartConnection(true);
        _tugboat.StartConnection(false);
        SceneLoadData sld = new SceneLoadData("Game");
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }
}