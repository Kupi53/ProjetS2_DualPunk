using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;


public class PointerScript : NetworkBehaviour
{
    [SerializeField] private Sprite PointerBase;
    [SerializeField] private Sprite Pointer1;
    [SerializeField] private Sprite Pointer2;
    [SerializeField] private Sprite Pointer3;
    [SerializeField] private Sprite Pointer4;
    [SerializeField] private Sprite SmartPointer1;
    [SerializeField] private Sprite SmartPointer2;
    [SerializeField] private Sprite SmartPointer3;
    [SerializeField] private Sprite SmartPointer4;
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
                spriteRenderer.sprite = PointerBase;
                break;

            case 1:
                ChangePointer(Pointer1, Pointer2, Pointer3, Pointer4);
                break;

            case 2:
                ChangePointer(SmartPointer1, SmartPointer2, SmartPointer3, SmartPointer4);
                break;
        }
    }


    private void ChangePointer(Sprite pointer1, Sprite pointer2, Sprite pointer3, Sprite pointer4)
    {
        if (!References.playerState.Walking)
        {
            if (target == null)
                spriteRenderer.sprite = pointer1;
            else
                spriteRenderer.sprite = pointer2;
        }
        else
        {
            if (target == null)
                spriteRenderer.sprite = pointer3;
            else
                spriteRenderer.sprite = pointer4;
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