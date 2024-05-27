using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PromptController : MonoBehaviour
{
    public Prompt Prompt;
    private Queue<string> _textToDisplay;
    private string _currentText {get => _textToDisplay.Peek();}
    private bool _firstFrame = true;

    public void Init()
    {
        _textToDisplay = new Queue<string>();
        foreach (string text in Prompt.TextFields)
        {
            _textToDisplay.Enqueue(text);
        }
        GameManager.Instance.LocalPlayer.GetComponent<MouvementsController>().EnableMovement = 
                GameManager.Instance.LocalPlayer.GetComponent<MouvementsController>().EnableMovement && Prompt.EnableMovement;
    }
    void Update()
    {
        gameObject.GetComponentInChildren<TMP_Text>().text = _currentText;
        if (Prompt.PromptType is not PromptType.Unclosable && Input.GetButtonDown("Interact") && !_firstFrame)
        {
            if (_textToDisplay.Count > 1)
            {
                _textToDisplay.Dequeue();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        _firstFrame = false;
    }
}