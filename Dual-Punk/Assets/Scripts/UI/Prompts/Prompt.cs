using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Prompt
{
    public string[] TextFields;
    public DialogueField[] DialogueFields;
    public bool EnableMovement;
    public PromptType PromptType;
    public GameObject Trigger;
}

public enum PromptType
{
    Dialogue,
    Unclosable,
    Closable,
}
