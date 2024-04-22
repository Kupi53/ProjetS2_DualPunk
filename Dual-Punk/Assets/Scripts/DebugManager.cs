using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private GameObject _weaponToSpawn;


    void Start()
    {   
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        if (GameManager.Instance == null) return;


        if (Input.GetKeyDown(KeyCode.H))
        {
            GameManager.Instance.DebugMode = true;
            Debug.Log("Debug Mode Activated");
        }
        if (!GameManager.Instance.DebugMode) return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            ObjectSpawner.Instance.SpawnObjectRpc(_weaponToSpawn, GameManager.Instance.Player1.transform.position, Quaternion.identity);
        }
    }
}
