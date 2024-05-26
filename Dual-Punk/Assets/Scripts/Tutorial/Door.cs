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

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        _playersOnDoor = new List<GameObject>();
        _targetPosition = _target.transform.GetChild(0).transform.position;
        Debug.Log(_targetPosition);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_playersOnDoor.Contains(other.gameObject))
        {
            _playersOnDoor.Add(other.gameObject);
        }
        if (_playersOnDoor.Count < 2)
        {
            if (IsServer)
            {
                SpawnLocalPrompt(other.gameObject.GetComponent<NetworkObject>().LocalConnection, PromptType.UnclosablePrompt, "you need to wait for second player :)");
            }
        }
        else
        {
            if (GameManager.Instance.CurrentPromptShown != null)
            {
                Destroy(GameManager.Instance.CurrentPromptShown);
            }
            GameManager.Instance.SpawnPrompt(PromptType.UnclosablePrompt, "press e to tp");
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (Input.GetButtonDown("Pickup") && _playersOnDoor.Count == 2)
        {
            TeleportRPC();
        }
    }



    void OnTriggerExit2D(Collider2D other)
    {
        _playersOnDoor.Remove(other.gameObject);
        if (GameManager.Instance.CurrentPromptShown != null)
        {
            Destroy(GameManager.Instance.CurrentPromptShown);
        }
    }

    [TargetRpc]
    void SpawnLocalPrompt(NetworkConnection con, PromptType type, string text)
    {
        GameManager.Instance.SpawnPrompt(type, text);
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
