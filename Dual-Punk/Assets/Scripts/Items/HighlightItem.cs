using UnityEngine;


public class HighlightItem : MonoBehaviour
{
    [SerializeField] private Sprite normal;
    [SerializeField] private Sprite highlighted;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public bool selectedWeapon;

    void Update()
    {
        if(!selectedWeapon){
            spriteRenderer.sprite = normal;
        }

    }

    public void Highlight()
    {
        spriteRenderer.sprite = highlighted;
        selectedWeapon = true;
        Debug.Log("HEEEERRRREEEEE");
    }
}