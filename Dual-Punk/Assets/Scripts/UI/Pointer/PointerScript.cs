using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using UnityEngine.UI;


public class PointerScript : MonoBehaviour
{
    private PlayerState _playerState;

    private void Start()
    {
        _playerState = transform.root.GetComponent<LocalPlayerReference>().PlayerState;
    }

    private void Update()
    {
        transform.position = _playerState.MousePosition;
    }
}