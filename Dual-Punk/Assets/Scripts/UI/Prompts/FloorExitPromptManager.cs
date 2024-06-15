using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class FloorExitPromptManager : MonoBehaviour
{
    List<GameObject> _playersOnExit;
    PromptTrigger[] _promptTriggers;
    int _lessThanTwoPromptIndex;
    int _twoPromptIndex;
    void Start()
    {
        _playersOnExit = new List<GameObject>();
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_playersOnExit.Contains(other.gameObject))
        {
            _playersOnExit.Add(other.gameObject);
            if (GameManager.Instance.Solo) return;
            if (_playersOnExit.Count == 2)
            {
                _promptTriggers[_lessThanTwoPromptIndex].OnTriggerExit2D(GameManager.Instance.LocalPlayer.GetComponent<Collider2D>());
                _promptTriggers[_lessThanTwoPromptIndex].enabled = false;
                _promptTriggers[_twoPromptIndex].enabled = true;
                _promptTriggers[_twoPromptIndex].OnTriggerEnter2D(GameManager.Instance.LocalPlayer.GetComponent<Collider2D>());
            }
        }
    }    
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<NetworkObject>().Owner == GameManager.Instance.LocalPlayer.GetComponent<NetworkObject>().Owner)

        _playersOnExit.Remove(other.gameObject);

        if (_promptTriggers[_twoPromptIndex].enabled && !GameManager.Instance.Solo)
        {
            _promptTriggers[_twoPromptIndex].OnTriggerExit2D(GameManager.Instance.LocalPlayer.GetComponent<Collider2D>());
            _promptTriggers[_twoPromptIndex].enabled = false;
            _promptTriggers[_lessThanTwoPromptIndex].enabled = true;
            _promptTriggers[_lessThanTwoPromptIndex].OnTriggerEnter2D(GameManager.Instance.LocalPlayer.GetComponent<Collider2D>());
        }
    }
}