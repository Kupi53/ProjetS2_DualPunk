using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using Unity.Mathematics;
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
        if (TutorialManager.Instance.CurrentStage < 8)
        {
            StartGameObservers(true);
        }
        else
        {
            StartGameObservers(false);
        }
    }

    [ObserversRpc]
    void StartGameObservers(bool skipped)
    {
        Destroy(GameObject.Find("Tutorial"));
        GameManager.Instance.FadeIn();
        GameManager.Instance.InTutorial = false;
        if (skipped)
        {
            ObjectSpawner.Instance.SpawnObjectFromIdRpc("0009", GameManager.Instance.LocalPlayer.transform.position, quaternion.identity);
            ObjectSpawner.Instance.SpawnObjectFromIdRpc("0020", GameManager.Instance.LocalPlayer.transform.position, quaternion.identity);
        }
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
