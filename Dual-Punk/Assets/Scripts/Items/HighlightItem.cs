using UnityEngine;


public class HighlightItem : MonoBehaviour
{
    [SerializeField] private Sprite _normal;
    [SerializeField] private Sprite _highlighted;

    private SpriteRenderer _spriteRenderer;
    public bool Selected { get; set; }


    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!Selected)
        {
            _spriteRenderer.sprite = _normal;
        }

    }

    public void Highlight()
    {
        _spriteRenderer.sprite = _highlighted;
        Selected = true;
    }
}