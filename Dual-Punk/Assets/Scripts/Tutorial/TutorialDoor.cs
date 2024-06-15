using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TutorialDoor : DoorBase
{
    [SerializeField]
    private GameObject _target;
    private Vector2 _targetPosition;


    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        _playersOnDoor = new List<GameObject>();
        _targetPosition = _target.transform.GetChild(0).transform.position;
    }


    [ServerRpc(RequireOwnership = false)]
    public override void DoActionRpc()
    {
        //Teleport Rpc
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
        //_target.tag = "ActiveRoom";
        //gameObject.tag = "Untagged";
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<NetworkObject>().Owner == GameManager.Instance.LocalPlayer.GetComponent<NetworkObject>().Owner)
            _onTrigger = false;

        _playersOnDoor.Remove(other.gameObject);

        if (_promptTriggers[_twoPromptIndex].enabled && !GameManager.Instance.Solo)
        {
            _promptTriggers[_twoPromptIndex].OnTriggerExit2D(GameManager.Instance.LocalPlayer.GetComponent<Collider2D>());
            _promptTriggers[_twoPromptIndex].enabled = false;
            _promptTriggers[_lessThanTwoPromptIndex].enabled = true;
            _promptTriggers[_lessThanTwoPromptIndex].OnTriggerEnter2D(GameManager.Instance.LocalPlayer.GetComponent<Collider2D>());
        }
    }
}
