using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _weaponsToSpawn;
    [SerializeField] private GameObject[] _implantsToSpawn;
    [SerializeField] private GameObject[] _enemiesToSpawn;
    [SerializeField] private GameObject _typeTextGO;
    [SerializeField] private GameObject _objectTextGO;

    private TextMeshProUGUI _typeText;
    private TextMeshProUGUI _objectText;
    private int _type;
    private int _index1;
    private int _index2;
    private int _index3;
    private Camera mainCamera;


    void Start()
    {   
        _type = 0;
        _index1 = 0;
        _index2 = 0;
        _index3 = 0;

        _typeText = _typeTextGO.GetComponent<TextMeshProUGUI>();
        _objectText = _objectTextGO.GetComponent<TextMeshProUGUI>();

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


        switch (_type)
        {
            case 0:
                _typeText.text = "Weapon :";
                _objectText.text = _weaponsToSpawn[_index1].name;
                break;
            case 1:
                _typeText.text = "Implant :";
                _objectText.text = _implantsToSpawn[_index2].name;
                break;
            case 2:
                _typeText.text = "Enemy :";
                _objectText.text = _enemiesToSpawn[_index3].name;
                break;
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            ObjectSpawner.Instance.SpawnObjectRpc(GetGameObject(), GameManager.Instance.Player1.GetComponent<PlayerState>().MousePosition, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            _type = (_type + 1) % 3;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            AssignIndex(1);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            AssignIndex(-1);
        }

        if (Input.GetKey(KeyCode.T))
        {
            GameManager.Instance.LocalPlayer.transform.position = GameManager.Instance.Player1.GetComponent<PlayerState>().MousePosition;
        }
        if (Input.GetKey(KeyCode.M))
        {
            foreach (GameObject Enemy in FloorNetworkWrapper.Instance.LocalFloorManager.CurrentRoom.Enemies)
            {
                Enemy.GetComponent<EnemyHealthManager>().SetHealth(0);
            }
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            GameManager.Instance.LocalPlayerState.IsDown = true;
        }
    }


    private void AssignIndex(int num)
    {
        switch (_type)
        {
            case 0:

                _index1 += num;
                if (_index1 < 0)
                    _index1 = _weaponsToSpawn.Length - 1;
                else
                    _index1 %= _weaponsToSpawn.Length;
                break;

            case 1:

                _index2 += num;
                if (_index2 < 0)
                    _index2 = _implantsToSpawn.Length - 1;
                else
                    _index2 %= _implantsToSpawn.Length;
                break;

            case 2:

                _index3 += num;
                if (_index3 < 0)
                    _index3 = _enemiesToSpawn.Length - 1;
                else
                    _index3 %= _enemiesToSpawn.Length;
                break;
        }
    }


    private GameObject GetGameObject()
    {
        switch (_type)
        {
            case 0:
                return _weaponsToSpawn[_index1];
            case 1:
                return _implantsToSpawn[_index2];
            case 2:
                return _enemiesToSpawn[_index3];
            default:
                return null;
        }
    }
}
