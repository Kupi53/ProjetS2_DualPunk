using System.Collections;
using System.Collections.Generic;
using FishNet.Managing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private GameObject networkManager;
    private RelayManager relayManager;
    void Start()
    {
        networkManager = GameObject.Find("NetworkManager");
        relayManager = networkManager.GetComponent<RelayManager>();
    }

    public void PlayGame()
    {
        /*Load la prochaine scene en recuperant la 
        scene actuelle et en generant la prochaine*/
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); 
    }

    public void ReturnToMenu()
    {
        if (networkManager.GetComponent<NetworkManager>().IsServer){
            if (GameManager.Instance.DebugMode){
                networkManager.GetComponent<NetworkManager>().ServerManager.StopConnection(true);
            }
            else {
                ServerConnectionsManager.Instance.CloseConnection();
            }
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
        else{
            relayManager.clientRequestedDisconnect = true;
            networkManager.GetComponent<NetworkManager>().ClientManager.StopConnection();
        }
        GameManager.Instance.InGame = false;
    }
}
