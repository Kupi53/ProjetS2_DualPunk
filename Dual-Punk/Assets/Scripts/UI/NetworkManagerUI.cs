using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private GameObject playerPrefabA; //add prefab in inspector
    [SerializeField] private GameObject playerPrefabB; //add prefab in inspector


    private void Awake(){
        hostButton.onClick.AddListener(() => {
            StartCoroutine(LoadGameScene("host"));
        });
        clientButton.onClick.AddListener(() => {
            StartCoroutine(LoadGameScene("client"));
        });
    }

// g volé ce script dans la docu et modifié il permet de changer de scene de maniere clean
    IEnumerator LoadGameScene(string type)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Game"));
        // spawn le joueur
        if (type=="host"){
            NetworkManager.Singleton.StartHost();
            /*GameObject player = Instantiate(playerPrefabA, new Vector3(0,0,0), Quaternion.identity).gameObject;
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);
            Debug.Log("host de merde");*/
            
        }
        else if (type=="client"){
            NetworkManager.Singleton.StartClient();
            /*Debug.Log("client de merde");
            SpawnServerRpc(NetworkManager.Singleton.LocalClientId);*/
        }
        // Unload the previous Scene
        SceneManager.UnloadSceneAsync("Menu");
    }

    /*[ServerRpc(RequireOwnership = false)]
    public void SpawnServerRpc(ulong clientId){
        Debug.Log("client de merde11");
        GameObject player = Instantiate(playerPrefabB, new Vector3(0,0,0), Quaternion.identity).gameObject;
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        Debug.Log("client de merde");
    }*/
}