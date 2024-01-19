using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;

public class PointerScript : MonoBehaviour
{
    public Sprite pointerNormal;
    public Sprite pointer1;
    public Sprite pointer2;
    public Sprite pointerAim1;
    public Sprite pointerAim2;
    public SpriteRenderer spriteRenderer;
    private bool onTarget;

    // Start is called before the first frame update
    void Start()
    {
        onTarget = false;
        ChangePosition();
    }

    // Update is called once per frame
    void Update()
    {
        ChangePosition();

        if (PlayerState.HoldingWeapon)
        {
            if (PlayerState.Aiming)
            {
                if (onTarget)
                {
                    spriteRenderer.sprite = pointerAim2;
                }
                else
                {
                    spriteRenderer.sprite = pointerAim1;
                }
            }
            else
            {
                if (onTarget)
                {
                    spriteRenderer.sprite = pointer2;
                }
                else
                {
                    spriteRenderer.sprite = pointer1;
                }
            }
        }
        else
        {
            spriteRenderer.sprite = pointerNormal;
        }
    }

    void ChangePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        mousePos.z = 0;
        transform.position = mousePos;
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<EnnemyScript>() != null)
        {
            onTarget = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<EnnemyScript>() != null)
        {
            onTarget = false;
        }
    }
}