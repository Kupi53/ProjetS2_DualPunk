using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using Unity.Netcode;

public class MaxHealth : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private PlayerState _playerState;

    void Start()
    {
        _playerState = transform.root.gameObject.GetComponent<LocalPlayerReference>().PlayerState;
    }

    void Update()
    {
        _text.text = _playerState.MaxHealth.ToString();
    }
}
