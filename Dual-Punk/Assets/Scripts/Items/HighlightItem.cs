using FishNet.Component.Animating;
using UnityEngine;


public class HighlightItem : MonoBehaviour
{
    [SerializeField] private Sprite _normal;
    [SerializeField] private Sprite _highlighted;

    private SpriteRenderer _spriteRenderer;
    private NetworkAnimator _animator;
    private string _currentAnim;


    private void Start()
    {
        _animator = GetComponentInChildren<NetworkAnimator>();
        if (_animator == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    public void StopHighlight()
    {
        if (_animator != null)
            ChangeAnim("drop");
        else
            _spriteRenderer.sprite = _normal;
    }

    public void Highlight()
    {
        if (_animator != null)
            ChangeAnim("dropHighlight");
        else
            _spriteRenderer.sprite = _highlighted;
    }

    private void ChangeAnim(string newAnim)
    {
        if (newAnim != _currentAnim)
        {
            _currentAnim = newAnim;
            _animator.Play(newAnim);
        }
    }
}