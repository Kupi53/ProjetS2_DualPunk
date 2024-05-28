using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class EndOfTutorialTrigger : NetworkBehaviour
{
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
        _promptTriggers[_twoPromptIndex].enabled = false;
    }
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        _playersOnDoor = new List<GameObject>();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (GameManager.Instance.DebugMode)
        {
            StartGameRpc();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_playersOnDoor.Contains(other.gameObject))
        {
            _playersOnDoor.Add(other.gameObject);
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
        if (Input.GetButtonDown("Pickup") && _playersOnDoor.Count == 2)
        {
            StartGameRpc();
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        _playersOnDoor.Remove(other.gameObject);
        _promptTriggers[_lessThanTwoPromptIndex].enabled = true;
        _promptTriggers[_twoPromptIndex].enabled = false;
        _promptTriggers[_lessThanTwoPromptIndex].OnTriggerExit2D(other);
    }

    [ServerRpc (RequireOwnership = false)]
    void StartGameRpc()
    {
        FloorNetworkWrapper.Instance.NewFloor(FloorType.City);
        StartGameObservers();
    }

    [ObserversRpc]
    void StartGameObservers()
    {
        Destroy(GameObject.Find("Tutorial"));
        GameManager.Instance.FadeIn();
        GameManager.Instance.LocalPlayer.transform.position = 
        FloorNetworkWrapper.Instance.LocalFloorManager.CurrentFloor.Entry.GetComponent<Grid>().CellToWorld(RoomExitTile.ComputeTargetCoordinates(
        FloorNetworkWrapper.Instance.LocalFloorManager.CurrentFloor.Entry._entryWallCardinal, 
        FloorNetworkWrapper.Instance.LocalFloorManager.CurrentFloor.Entry));
        GameManager.Instance.InTutorial = false;
    }
}
