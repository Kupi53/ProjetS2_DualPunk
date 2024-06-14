using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool DebugMode;
    public bool Solo;
    public bool InGame;
    public bool InTutorial;
    public GameObject Player1;
    public GameObject Player2;
    public bool downTutorialBegan;
    public PromptTrigger downTutorialPromptTrigger;
    public GameObject LocalPlayer {
        get
        {
            GameObject camera;
            PlayerState state;
            // REALLY degueulasse car ca a rien a voir avec la camera mais en gros ca return le joueur du client qui appelle la fonction car la camera est que en local
            if ((camera = GameObject.FindGameObjectWithTag("Camera")) == null) return null;
            else
            {
                if ((state = camera.GetComponent<CameraController>().PlayerState) == null) return null;
                else return state.gameObject;
            }
        } 
    }
    public PlayerState LocalPlayerState;
    public PlayerState Player1State;
    public PlayerState Player2State;

    void Start(){
        Player1 = null;
        Player2 = null;

        if (Instance == null)
        {
            Instance = this;
        }
        downTutorialPromptTrigger = gameObject.GetComponent<PromptTrigger>();
    }

    void Update()
    {
        if (Solo)
        {
            CheckDeathSolo();
        }
        else
        {
            CheckDeathMulti();
        }
    }

    public void FadeIn()
    {
        GameObject.Find($"{LocalPlayer.name} UI").GetComponentsInChildren<Animator>().Where(c=>c.gameObject.name == "FadeToBlack").First().Play("FadeToBlack_second");
    }
    public void FadeInDeath()
    {
        GameObject.Find($"{LocalPlayer.name} UI").GetComponentsInChildren<Animator>().Where(c=>c.gameObject.name == "DeathScreen").First().Play("FadeInDeath");
    }

    public void CheckDeathMulti()
    {
        if (Player1State == null || Player2State == null) return;
        else
        {
            if (Player1State.IsDown && Player2State.IsDown)
            {
                Lose();
            }
            else
            {
                if (Player1State.IsDown != Player2State.IsDown && !downTutorialBegan)
                {
                    PromptManager.Instance.SpawnPrompt(downTutorialPromptTrigger.Prompt, downTutorialPromptTrigger.gameObject);
                    downTutorialBegan = true;
                }
                else
                {
                    if (downTutorialBegan && !Player1State.IsDown && !Player2State.IsDown)
                    {
                        PromptManager.Instance.ClosePrompt(downTutorialPromptTrigger.Prompt);
                    }
                }
            }
        } 
    }

    public void CheckDeathSolo()
    {
        if (LocalPlayer == null) return;
        else
        {
            if (LocalPlayerState == null)
            {
                LocalPlayerState = LocalPlayer.GetComponent<PlayerState>();
            }
            if (LocalPlayerState.IsDown)
            {
                Lose();
            }
        }
    }

    public void Lose()
    {
        Debug.Log("Mort");
        Time.timeScale = 0;
        PromptManager.Instance.ClearPrompts();
        PromptManager.Instance.CloseCurrentArrow();
        PromptManager.Instance.CloseCurrentIndicator();
        FadeInDeath();
    }

    public void Reset()
    {
        InGame = false;
        downTutorialBegan = false;
        Solo = false;
        Player1 = null;
        Player2 = null;
        Player1State = null;
        Player2State = null;
        PromptManager.Instance.ClearPrompts();
        PromptManager.Instance.CloseCurrentArrow();
        PromptManager.Instance.CloseCurrentIndicator();
        Time.timeScale = 1;
    }
}