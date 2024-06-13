using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PromptController : MonoBehaviour
{
    public Prompt Prompt;
    private Queue<string> _textToDisplay;
    private GameObject Next;
    private string _currentText {get => _textToDisplay.Peek();}
    private bool _firstFrame = true;

    public void Init()
    {
        _textToDisplay = new Queue<string>();
        Next = GetComponentInChildren<Button>().gameObject;
        foreach (string text in Prompt.TextFields)
        {
            _textToDisplay.Enqueue(text);
        }
        GameManager.Instance.LocalPlayer.GetComponent<MouvementsController>().EnableMovement = 
                GameManager.Instance.LocalPlayer.GetComponent<MouvementsController>().EnableMovement && Prompt.EnableMovement;
        if (Prompt.PromptType == PromptType.Unclosable && _textToDisplay.Count < 2)
        {
           Next.SetActive(false);
        }
    }
    void Update()
    {
        gameObject.GetComponentInChildren<TMP_Text>().text = _currentText;
        if (Input.GetButtonDown("Interact") && !_firstFrame)
        {
            if (_textToDisplay.Count > 1)
            {
                _textToDisplay.Dequeue();
                if (Prompt.PromptType == PromptType.Unclosable)
                {
                    if (_textToDisplay.Count > 1 )
                    {
                        Next.SetActive(true);
                    }
                    else Next.SetActive(false);
                }

            }
            else
            {
                if (Prompt.PromptType != PromptType.Unclosable) Destroy(gameObject);
            }
        }
        _firstFrame = false;
    }
}