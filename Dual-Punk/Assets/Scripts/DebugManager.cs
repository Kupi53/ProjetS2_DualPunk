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
        if (Input.GetKey(KeyCode.AltGr))
        {
            GameManager.Instance.DebugMode = true;
            Debug.Log("Debug Mode Activated");
        }
        if (!GameManager.Instance.DebugMode) return;

        if (Input.GetKey(KeyCode.P))
        {
            ObjectSpawner.Instance.SpawnObjectRpc(_weaponToSpawn, GameManager.Instance.Player1.transform.position, new Quaternion());
        }
    }
}
