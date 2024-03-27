using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FishNet.Object;

public class CurrentHealth : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private PlayerState _playerState;

    void Start()
    {
        _playerState = gameObject.transform.root.gameObject.GetComponent<LocalPlayerReference>().PlayerState;
    }

    void Update()
    {
        _text.text = _playerState.Health.ToString();
    }
}
