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


    private void Awake(){
        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Additive);
            NetworkManager.Singleton.StartHost();

        });
        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Additive);
            NetworkManager.Singleton.StartClient();
        });
    }
}