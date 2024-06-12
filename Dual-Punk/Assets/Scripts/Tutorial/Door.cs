using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Door : NetworkBehaviour
{
    [SerializeField] GameObject _target;
    Vector2 _targetPosition;
    List<GameObject> _playersOnDoor;
    PromptTrigger[] _promptTriggers;
    int _lessThanTwoPromptIndex;
    int _twoPromptIndex;

    void Start()
    {
        _promptTriggers = gameObject.GetComponents<PromptTrigger>();
        if (_promptTriggers[0].Prompt.TextFields[0].StartsWith("Wait")) 
        {
            _lessThanTwoPromptIndex = 0;
            _twoPromptIndex = 1;
        }
        else
        {
            _lessThanTwoPromptIndex = 1;
            _twoPromptIndex = 0;
        }
        if (GameManager.Instance.Solo) _promptTriggers[_lessThanTwoPromptIndex].enabled = false;
        else _promptTriggers[_twoPromptIndex].enabled = false;
    }
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        _playersOnDoor = new List<GameObject>();
        _targetPosition = _target.transform.GetChild(0).transform.position;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_playersOnDoor.Contains(other.gameObject))
        {
            _playersOnDoor.Add(other.gameObject);
            if (GameManager.Instance.Solo) return;
            if (_playersOnDoor.Count == 2)
            {
                _promptTriggers[_lessThanTwoPromptIndex].OnTriggerExit2D(GameManager.Instance.LocalPlayer.GetComponent<Collider2D>());
                _promptTriggers[_lessThanTwoPromptIndex].enabled = false;
                _promptTriggers[_twoPromptIndex].enabled = true;
                _promptTriggers[_twoPromptIndex].OnTriggerEnter2D(GameManager.Instance.LocalPlayer.GetComponent<Collider2D>());
            }
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        bool canTp;
        if (GameManager.Instance.Solo) canTp = true;
        else canTp = _playersOnDoor.Count == 2;
        if (Input.GetButtonDown("Pickup") && canTp)
        {
            TeleportRPC();
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        _playersOnDoor.Remove(other.gameObject);
        if (GameManager.Instance.Solo) return;
        if (_promptTriggers[_twoPromptIndex].enabled)
        {
            _promptTriggers[_lessThanTwoPromptIndex].enabled = true;
            _promptTriggers[_twoPromptIndex].enabled = false;
            _promptTriggers[_lessThanTwoPromptIndex].OnTriggerExit2D(other);
        }
    }

    [ServerRpc (RequireOwnership = false)]
    void TeleportRPC()
    {
        TeleportLocationObservers();
        SetNextRoomCurrentObservers();
    }

    [ObserversRpc]
    void TeleportLocationObservers()
    {
        GameManager.Instance.LocalPlayer.transform.position = _targetPosition;
    }
    [ObserversRpc]
    void SetNextRoomCurrentObservers()
    {
        _target.tag = "ActiveRoom";
        gameObject.tag = "Untagged";
    }
}
