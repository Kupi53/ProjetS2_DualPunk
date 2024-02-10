using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Netcode.Transports.UTP;

public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button connectButton;
    [SerializeField] private TMP_InputField ipField;

    private void Awake(){
        hostButton.onClick.AddListener(() => {
            StartCoroutine(LoadLobbyScene("host"));
        });
        connectButton.onClick.AddListener(()=>{
            StartCoroutine(LoadLobbyScene("client"));
        });
    }
// g volé ce script dans la docu et modifié il permet de changer de scene de maniere clean
    IEnumerator LoadLobbyScene(string type)
    {
        // lobby scene
        if (type=="host"){
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Additive);
            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            SceneManager.UnloadSceneAsync("Menu");
            NetworkManager.Singleton.StartHost();
        }
        if (type == "client"){
            NetworkManager.GetComponent<UnityTransport>().ConnectionData.Address = ipField.text;
            if (NetworkManager.Singleton.StartClient()){
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Additive);
                // Wait until the asynchronous scene fully loads
                while (!asyncLoad.isDone)
                {
                    yield return null;
                }
                SceneManager.UnloadSceneAsync("Menu");
            }
        }
    }

    public void OnIpInputChanged(){
        if (ipField.text == ""){
            connectButton.interactable = false;
            connectButton.GetComponentInChildren<TMP_Text>().color = new Color32(0, 185, 255, 100);
        }
        else{
            connectButton.interactable = true;
            connectButton.GetComponentInChildren<TMP_Text>().color = new Color32(0, 185, 255, 255);

        }
    }
    /*[ServerRpc(RequireOwnership = false)]
    public void SpawnServerRpc(ulong clientId){
        Debug.Log("client de merde11");
        GameObject player = Instantiate(playerPrefabB, new Vector3(0,0,0), Quaternion.identity).gameObject;
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        Debug.Log("client de merde");
    }*/
}