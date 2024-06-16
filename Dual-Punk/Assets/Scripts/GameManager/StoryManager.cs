using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;


public class StoryManager : NetworkBehaviour
{
    [SerializeField] StoryMonologue[] _storyMonologues;
    [SerializeField] Sprite _storyNpcSprite;
    [SerializeField] string _storyNpcName;
    [SerializeField] GameObject _storyNpc;
    public static StoryManager Instance;
    public bool StoryCompleted
    {
        get
        {
            return _currentStoryIndex >= _storyMonologues.Length;
        }
    }
    private int _currentStoryIndex;
    private bool _firstNpcSpawned;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _currentStoryIndex = 0;
    }

    public void SpawnNpc(Vector3 position)
    {
        if (!IsServer || _currentStoryIndex >= _storyMonologues.Length || (_currentStoryIndex>5 && FloorNetworkWrapper.Instance.LocalFloorManager.CurrentFloor.FloorType == FloorType.City )) return;
        GameObject storyNpc = Instantiate(_storyNpc, position, Quaternion.identity);
        Spawn(storyNpc);
        InitPromptObs(storyNpc);
        ObjectSpawner.Instance.ObjectParentToRoomRpc(storyNpc);
        if (!_firstNpcSpawned)
        {
            SpawnPointerArrowObs(storyNpc);
        }
    }

    [ObserversRpc]
    void InitPromptObs(GameObject npc)
    {
        Prompt prompt = npc.GetComponentInChildren<PromptTrigger>().Prompt;
        int nbDialogueFields = _storyMonologues[_currentStoryIndex].text.Length;
        prompt.DialogueFields = new DialogueField[nbDialogueFields];
        for (int i = 0; i<nbDialogueFields;i++)
        {
            string text = _storyMonologues[_currentStoryIndex].text[i];
            prompt.DialogueFields[i] = new DialogueField(_storyNpcSprite, _storyNpcName, text);
        }
        _currentStoryIndex += 1;
    }

    [ObserversRpc]
    void SpawnPointerArrowObs(GameObject storyNpc)
    {
        PromptManager.Instance.SpawnPointerArrow(storyNpc);
    }
}

[System.Serializable]
internal class StoryMonologue
{
    [SerializeField] public string[] text;
}