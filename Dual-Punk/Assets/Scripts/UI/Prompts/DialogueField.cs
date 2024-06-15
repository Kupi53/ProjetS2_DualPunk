using UnityEngine;

[System.Serializable]
public class DialogueField
{
    [SerializeField] public Sprite Portrait;
    [SerializeField] public string Name;
    [SerializeField] public string Text;

    public DialogueField(Sprite portrait, string name, string text)
    {
        Portrait = portrait;
        Name = name;
        Text = text;
    }

}