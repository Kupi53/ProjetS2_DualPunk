using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;


public class PointerScript : MonoBehaviour
{
    [SerializeField] private Sprite pointerNormal;
    [SerializeField] private Sprite pointer1;
    [SerializeField] private Sprite pointer2;
    [SerializeField] private Sprite pointerAim1;
    [SerializeField] private Sprite pointerAim2;
    [SerializeField] private Sprite autolockPointer;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private PlayerState playerState;
    private Vector2 mousePos;
    private bool onTarget;


    void Start()
    {
        onTarget = false;
        playerState = transform.root.gameObject.GetComponent<LocalPlayerReference>().playerState;
    }


    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePos;

        switch (playerState.pointer)
        {
            case -1:
                spriteRenderer.enabled = false;
                break;

            case 0:
                spriteRenderer.sprite = pointerNormal;
                break;

            case 1:
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
                break;

            case 2:
                spriteRenderer.sprite = autolockPointer;
                break;
        }
    }


    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ennemy"))
        {
            onTarget = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ennemy"))
        {
            onTarget = false;
        }
    }
}