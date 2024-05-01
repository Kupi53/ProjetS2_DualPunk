using UnityEngine;


public class HighlightItem : MonoBehaviour
{
    [SerializeField] private Sprite normal;
    [SerializeField] private Sprite highlighted;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public bool selected;

    void Update()
    {
        if(!selected){
            spriteRenderer.sprite = normal;
        }

    }

    public void Highlight()
    {
        spriteRenderer.sprite = highlighted;
        selected = true;
    }
}