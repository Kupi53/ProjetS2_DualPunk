using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class DoorBase : NetworkBehaviour
{
    protected List<GameObject> _playersOnDoor;
    protected PromptTrigger[] _promptTriggers;
    protected int _lessThanTwoPromptIndex;
    protected int _twoPromptIndex;
    protected bool _onTrigger;


    private void Start()
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

        _onTrigger = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact") && _onTrigger && (GameManager.Instance.Solo || _playersOnDoor.Count == 2))
        {
            DoActionRpc();
        }
    }


    public abstract void DoActionRpc();


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<NetworkObject>().Owner == GameManager.Instance.LocalPlayer.GetComponent<NetworkObject>().Owner)
            _onTrigger = true;

        if (!_playersOnDoor.Contains(other.gameObject))
        {
            _playersOnDoor.Add(other.gameObject);

            if (_playersOnDoor.Count == 2 && !GameManager.Instance.Solo)
            {
                _promptTriggers[_lessThanTwoPromptIndex].OnTriggerExit2D(GameManager.Instance.LocalPlayer.GetComponent<Collider2D>());
                _promptTriggers[_lessThanTwoPromptIndex].enabled = false;
                _promptTriggers[_twoPromptIndex].enabled = true;
                _promptTriggers[_twoPromptIndex].OnTriggerEnter2D(GameManager.Instance.LocalPlayer.GetComponent<Collider2D>());
            }
        }
    }
}
