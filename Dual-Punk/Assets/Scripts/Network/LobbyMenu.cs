using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : NetworkBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private GameObject waiting;
    [SerializeField] private GameObject ready;
    // Start is called before the first frame update
    void Awake()
    {
        cancelButton.onClick.AddListener(() => {
            StartCoroutine(LoadMenu());
        });
        playButton.onClick.AddListener(()=>{
            StartCoroutine(LoadGame());
        });
    }

    void Update(){
        if (!IsServer) return;
        if (NetworkManager.Singleton.ConnectedClients.Count == 2){
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

    IEnumerator LoadMenu()
    {
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        SceneManager.UnloadSceneAsync("Lobby");
    }
    IEnumerator LoadGame()
    {
        NetworkManager.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        yield return new WaitForEndOfFrame();
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Game"));
        NetworkManager.SceneManager.UnloadScene(SceneManager.GetSceneByName("Lobby"));
    }

    [ClientRpc]
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