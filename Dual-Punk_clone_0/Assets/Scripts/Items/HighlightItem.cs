using System.Collections;
using System.Collections.Generic;
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