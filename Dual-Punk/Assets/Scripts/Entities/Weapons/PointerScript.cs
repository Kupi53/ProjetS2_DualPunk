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
    [SerializeField] private Sprite specialPointer1;
    [SerializeField] private Sprite specialPointer2;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private LocalPlayerReference References;

    public GameObject? target;
    public Vector3 position;
    public int spriteNumber;
    public bool locked;


    void Start()
    {
        References = transform.root.gameObject.GetComponent<LocalPlayerReference>();
        References.playerState.Pointer = gameObject;
    }


    void Update()
    {
        if (locked && target != null)
        {
            transform.position = target.transform.position;
        }
        else
        {
            position = References.Camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            transform.position = position;
        }


        switch (spriteNumber)
        {
            case 0:
                spriteRenderer.sprite = pointerNormal;
                break;

            case 1:
                if (References.playerState.Walking)
                {
                    if (target != null)
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
                    if (target != null)
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
                if (target == null) spriteRenderer.sprite = specialPointer1;
                else spriteRenderer.sprite = specialPointer2;
                break;
        }
    }


    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ennemy"))
        {
            target = collision.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ennemy"))
        {
            target = null;
        }
    }
}