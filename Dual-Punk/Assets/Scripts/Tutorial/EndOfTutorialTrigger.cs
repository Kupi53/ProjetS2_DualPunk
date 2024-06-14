using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class EndOfTutorialTrigger : DoorBase
{
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        _playersOnDoor = new List<GameObject>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }


    [ServerRpc (RequireOwnership = false)]
    public override void DoActionRpc()
    {
        //Start Game Rpc
        FloorNetworkWrapper.Instance.NewFloor(FloorType.City);
        StartGameObservers();
    }

    [ObserversRpc]
    void StartGameObservers()
    {
        Destroy(GameObject.Find("Tutorial"));
        GameManager.Instance.FadeIn();
        GameManager.Instance.InTutorial = false;
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<NetworkObject>().Owner == GameManager.Instance.LocalPlayer.GetComponent<NetworkObject>().Owner)
            _onTrigger = false;

        _playersOnDoor.Remove(other.gameObject);

        if (GameManager.Instance.Solo) return;

        _promptTriggers[_lessThanTwoPromptIndex].enabled = true;
        _promptTriggers[_twoPromptIndex].enabled = false;
        _promptTriggers[_lessThanTwoPromptIndex].OnTriggerExit2D(other);
    }
}
