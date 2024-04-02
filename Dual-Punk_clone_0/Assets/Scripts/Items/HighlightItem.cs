using UnityEngine;

public class HighlightItem : MonoBehaviour
{
    [SerializeField] private Sprite normal;
    [SerializeField] private Sprite highlighted;
    [SerializeField] private SpriteRenderer spriteRenderer;

    void Update()
    {
        spriteRenderer.sprite = normal;
    }

    public void Highlight()
    {
        spriteRenderer.sprite = highlighted;
    }
}