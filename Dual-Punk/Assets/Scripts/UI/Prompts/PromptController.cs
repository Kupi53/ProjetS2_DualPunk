using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class PromptController : MonoBehaviour
{
    public Prompt Prompt;
    private Queue<string> _textToDisplay;
    private Queue<DialogueField> _dialogueToDisplay;
    private GameObject Next;
    private string _currentText {get => _textToDisplay.Peek();}
    private DialogueField _currentDialogue {get=> _dialogueToDisplay.Peek();}
    private bool _firstFrame = true;
    private TMP_Text _textField;
    # nullable enable
    private TMP_Text? _nameField;
    private Image? _portraitField;
    # nullable disable

    public void Init()
    {
        _textToDisplay = new Queue<string>();
        _dialogueToDisplay = new Queue<DialogueField>();
        Next = GetComponentInChildren<Button>().gameObject;
        _textField = GetComponentsInChildren<TMP_Text>().Where(c => c.gameObject.name == "Text").First();
        if (Prompt.PromptType == PromptType.Dialogue)
        {
            _nameField = GetComponentsInChildren<TMP_Text>().Where(c => c.gameObject.name == "Name").First();
            _portraitField = GetComponentsInChildren<Image>().Where(c => c.gameObject.name == "Portrait").First();
            foreach (DialogueField field in Prompt.DialogueFields)
            {
                _dialogueToDisplay.Enqueue(field);
            }
        }
        else
        {
            foreach (string text in Prompt.TextFields)
            {
                _textToDisplay.Enqueue(text);
            } 
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
        if (Prompt.PromptType == PromptType.Dialogue)
        {
            _textField.text = _currentDialogue.Text;
            _nameField.text = _currentDialogue.Name;
            _portraitField.sprite = _currentDialogue.Portrait;
        }
        else
        {
            _textField.text = _currentText;
        }
        if (Input.GetButtonDown("Interact") && !_firstFrame)
        {
            if (Prompt.PromptType == PromptType.Dialogue)
            {
                if (_dialogueToDisplay.Count > 1)
                {
                    _dialogueToDisplay.Dequeue();
                }
                else
                {
                    PromptManager.Instance.ClosePrompt(Prompt);
                }
            }
            else
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
                    if (Prompt.PromptType != PromptType.Unclosable) PromptManager.Instance.ClosePrompt(Prompt);
                }
            }
        }
        _firstFrame = false;
    }
}