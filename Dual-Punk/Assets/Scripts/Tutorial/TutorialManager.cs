using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FishNet.Object;
using FishNet.Utility.Extension;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    [SerializeField] GameObject _stageObjects;
    [SerializeField] GameObject _exteriorDoor;
    [SerializeField] GameObject _interiorDoor;
    public int CurrentStage;
    private bool _started;
    private float _stage1counter;
    private int _stage2counter;
    private bool _stage2dashToggle;
    private GameObject _stage3enemy;
    private GameObject _stage4lootBox;
    private bool _stage4cleared;

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
        switch (CurrentStage)
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
                    if (_stage2dashToggle)
                    {
                        _stage2dashToggle = false;
                        _stage2counter -= 1;
                        if (_stage2counter == 0)
                        {
                            ChangeStage(2,3);
                        }
                    }
                }
                else
                {
                    if (!_stage2dashToggle)
                    {
                        _stage2dashToggle = true;
                    }
                }
                break;
            case 3: 
                if (!PromptManager.Instance.CurrentArrowShown.GetComponent<Image>().enabled)
                {
                    Debug.Log(_stage4cleared);
                    if (!_stage4cleared) ChangeStage(3,4);
                    else ChangeStage(3,5);

                }
                break;
            case 4:
                if (PromptManager.Instance.CurrentArrowShown.GetComponent<Image>().enabled)
                {
                    ChangeStage(4,3);
                }
                else
                {
                    if (_stage4lootBox == null)
                    {
                        _stage4cleared = true;
                        ChangeStage(4,5);
                    }
                }
                break;
            case 5:
                if (PromptManager.Instance.CurrentArrowShown.GetComponent<Image>().enabled)
                {
                    ChangeStage(5,3);
                }
                else
                {
                    if (_stage3enemy == null)
                    {
                        ChangeStage(5,6);
                    }
                }
                break;
            case 6:
                if (GameManager.Instance.LocalPlayer.transform.position == _interiorDoor.transform.position)
                {
                    Debug.Log("vu");
                    ChangeStage(6,7);
                }
                break;
            case 7:
                if (GameManager.Instance.LocalPlayer.transform.position == _exteriorDoor.transform.position)
                {
                    Debug.Log("vu2");
                    ChangeStage(7,6);
                }
                break;
        }
    }
    void ChangeStage(int oldStage, int newStage)
    {
        CurrentStage = newStage;
        switch(CurrentStage)
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
                _stage2dashToggle = true;
                _stage4cleared = false;
                PromptManager.Instance.CloseCurrentPrompt();
                _stageObjects.transform.GetChild(2).GetComponentInChildren<PromptTrigger>().Spawn();
                break;
            case 3:
                _stage3enemy = FindClosestOfTwoTagged("Ennemy");
                PromptManager.Instance.CloseCurrentPrompt();
                PromptManager.Instance.CloseCurrentArrow();
                PromptManager.Instance.SpawnPointerArrow(_stage3enemy);
                _stageObjects.transform.GetChild(3).GetComponentInChildren<PromptTrigger>().Spawn();
                break;
            case 4:
                _stage4lootBox = FindClosestOfTwoTagged("Lootbox");
                PromptManager.Instance.CloseCurrentPrompt();
                _stageObjects.transform.GetChild(4).GetComponentInChildren<PromptTrigger>().Spawn();
                break;
            case 5:
                PromptManager.Instance.CloseCurrentPrompt();
                _stageObjects.transform.GetChild(5).GetComponentInChildren<PromptTrigger>().Spawn();
                break;
            case 6:
                PromptManager.Instance.CloseCurrentPrompt();
                PromptManager.Instance.CloseCurrentArrow();
                PromptManager.Instance.SpawnPointerArrow(_exteriorDoor);
                _stageObjects.transform.GetChild(6).GetComponentInChildren<PromptTrigger>().Spawn();
                break;
            case 7:
                PromptManager.Instance.CloseCurrentPrompt();
                PromptManager.Instance.CloseCurrentArrow();
                _stageObjects.transform.GetChild(7).GetComponentInChildren<PromptTrigger>().Spawn();
                break;
        }
    }

    GameObject FindClosestOfTwoTagged(string tag)
    {
        UnityEngine.Vector3 playerPos = GameManager.Instance.LocalPlayer.transform.position;
        GameObject[] obj = GameObject.FindGameObjectsWithTag(tag);
        UnityEngine.Vector3 coords1 = playerPos - obj[0].transform.position;
        UnityEngine.Vector3 coords2 = playerPos - obj[1].transform.position;
        if (Math.Abs(coords1.x) + Math.Abs(coords1.y) + Math.Abs(coords1.z) <= Math.Abs(coords2.x) + Math.Abs(coords2.y) + Math.Abs(coords2.z))
        {
            return obj[0];
        }
        else return obj[1];
    }

}