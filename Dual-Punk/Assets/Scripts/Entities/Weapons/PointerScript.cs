using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Properties;
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
    private PlayerState playerState;

    private bool onTarget;
    private Vector3 mousePos;


    void Start()
    {
        onTarget = false;
        playerState = gameObject.transform.root.gameObject.GetComponent<LocalPlayerReference>().playerState;
    }


    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos;

        if (playerState.HoldingWeapon)
        {
            if (playerState.Walking)
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


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ennemy"))
        {
            onTarget = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ennemy"))
        {
            onTarget = false;
        }
    }
}