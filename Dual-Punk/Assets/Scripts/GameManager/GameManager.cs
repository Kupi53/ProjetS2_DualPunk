using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private Vector2 _DEFAULTPROMPTPOSITION = new Vector2(0,-400);
    public static GameManager Instance;
    public GameObject CurrentPromptShown;
    public bool DebugMode;
    public bool InGame;
    public GameObject Player1;
    public GameObject Player2;
    public GameObject LocalPlayer {
        get
        {
            // REALLY degueulasse car ca a rien a voir avec la camera mais en gros ca return le joueur du client qui appelle la fonction car la camera est que en local
            return GameObject.FindGameObjectWithTag("Camera").GetComponent<CameraController>().PlayerState.gameObject;
        } 
    }

    void Start(){
        Player1 = null;
        Player2 = null;
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void FadeIn()
    {
        GameObject.Find($"{LocalPlayer.name} UI").GetComponentInChildren<Animator>().Play("FadeToBlack_second");
    }

    public bool SpawnPrompt(PromptType type, string text)
    {
        if (CurrentPromptShown == null)
        {
            GameObject Prompt;
            switch (type)
            {
                case PromptType.ClosablePrompt:
                    Prompt = (GameObject)Instantiate(Resources.Load("ClosablePrompt"), _DEFAULTPROMPTPOSITION, quaternion.identity);
                    break;
                case PromptType.UnclosablePrompt:
                    Prompt = (GameObject)Instantiate(Resources.Load("UnclosablePrompt"), _DEFAULTPROMPTPOSITION, quaternion.identity);
                    break;
                case PromptType.DialoguePrompt:
                    Prompt = (GameObject)Instantiate(Resources.Load("DialoguePrompt"), _DEFAULTPROMPTPOSITION, quaternion.identity);
                    break;
                default:
                    throw new System.Exception("prompt type not implemented");
            }
            if (Prompt != null)
            {
                CurrentPromptShown = Prompt;
                Prompt.GetComponentInChildren<TMP_Text>().text = text;
                Prompt.transform.SetParent(GameObject.Find("Canvas").transform, false);
            }
            return true;
        }
        else return false;
    }
    public bool SpawnPrompt(PromptType type, string text, Vector2 position)
    {
        if (CurrentPromptShown == null)
        {
            GameObject Prompt;
            switch (type)
            {
                case PromptType.ClosablePrompt:
                    Prompt = (GameObject)Instantiate(Resources.Load("ClosablePrompt"), position, quaternion.identity);
                    break;
                case PromptType.UnclosablePrompt:
                    Prompt = (GameObject)Instantiate(Resources.Load("UnclosablePrompt"), position, quaternion.identity);
                    break;
                case PromptType.DialoguePrompt:
                    Prompt = (GameObject)Instantiate(Resources.Load("DialoguePrompt"), position, quaternion.identity);
                    break;
                default:
                    throw new System.Exception("prompt type not implemented");
            }
            if (Prompt != null)
            {
                CurrentPromptShown = Prompt;
                Prompt.GetComponentInChildren<TMP_Text>().text = text;
                Prompt.transform.SetParent(GameObject.Find("Canvas").transform, false);
            }
            return true;
        }
        else return false;
    }
}

public enum PromptType
{
    ClosablePrompt,
    UnclosablePrompt,
    DialoguePrompt
}