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
        }
        else if (type=="client"){
            NetworkManager.Singleton.StartClient();
        }
        // Unload the previous Scene
        SceneManager.UnloadSceneAsync("Menu");
    }
}