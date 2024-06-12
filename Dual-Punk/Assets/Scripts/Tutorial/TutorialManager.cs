using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    [SerializeField] GameObject _stageObjects;
    private int _currentStage;
    private bool _started;
    private float _stage1counter;
    private int _stage2counter;
    private bool _dashToggle;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (!_started)
        {
            if (GameManager.Instance.LocalPlayer != null)
            {
                _started = true;
                ChangeStage(0,0);
            }
        }
        else
        {
            CheckStageChange();
        }
    }

    void CheckStageChange()
    {
        switch (_currentStage)
        {
            case 0:
                if (PromptManager.Instance.CurrentPromptShown == null)
                {
                    ChangeStage(0, 1);
                }
                break;
            case 1:
                if (GameManager.Instance.LocalPlayer.GetComponent<PlayerState>().Moving)
                {
                    _stage1counter -= Time.deltaTime;
                    if (_stage1counter <= 0)
                    {
                        ChangeStage(1,2);
                    }
                }
                break;
            case 2:
                if (GameManager.Instance.LocalPlayer.GetComponent<PlayerState>().Dashing)
                {
                    if (_dashToggle)
                    {
                        _dashToggle = false;
                        _stage2counter -= 1;
                        if (_stage2counter == 0)
                        {
                            ChangeStage(2,3);
                        }
                    }
                }
                else
                {
                    if (!_dashToggle)
                    {
                        _dashToggle = true;
                    }
                }
                break;
        }
    }
    void ChangeStage(int oldStage, int newStage)
    {
        _currentStage = newStage;
        switch(_currentStage)
        {
            case 0:
                _stageObjects.transform.GetChild(0).GetComponentInChildren<PromptTrigger>().Spawn();
                break;
            case 1:
                _stage1counter = 3;
                PromptManager.Instance.CloseCurrentPrompt();
                _stageObjects.transform.GetChild(1).GetComponentInChildren<PromptTrigger>().Spawn();
                break;
            case 2:
                _stage2counter = 3;
                _dashToggle = true;
                PromptManager.Instance.CloseCurrentPrompt();
                _stageObjects.transform.GetChild(2).GetComponentInChildren<PromptTrigger>().Spawn();
                break;
            case 3:
                PromptManager.Instance.CloseCurrentPrompt();
                _stageObjects.transform.GetChild(3).GetComponentInChildren<PromptTrigger>().Spawn();
                break;
        }
    }

}