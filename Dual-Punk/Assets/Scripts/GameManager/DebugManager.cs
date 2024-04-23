using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _weaponsToSpawn;
    private int _index;


    void Start()
    {   
        _index = 0;
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

        if (Input.GetKeyDown(KeyCode.O))
        {
            _index = (_index + 1) % _weaponsToSpawn.Length;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            ObjectSpawner.Instance.SpawnObjectRpc(_weaponsToSpawn[_index], GameManager.Instance.Player1.transform.position, Quaternion.identity);
        }
    }
}
