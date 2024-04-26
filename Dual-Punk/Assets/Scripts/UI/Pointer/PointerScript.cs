using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.Netcode;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using UnityEngine.UI;


public class PointerScript : MonoBehaviour
{
    [SerializeField] private Sprite _pointerBase;
    [SerializeField] private Sprite _pointer1;
    [SerializeField] private Sprite _pointer2;
    [SerializeField] private Sprite _pointer3;
    [SerializeField] private Sprite _pointer4;
    [SerializeField] private Sprite _smartPointer1;
    [SerializeField] private Sprite _smartPointer2;
    [SerializeField] private Sprite _smartPointer3;
    [SerializeField] private Sprite _smartPointer4;
    [SerializeField] private Sprite _chargePointer1;
    [SerializeField] private Sprite _chargePointer2;
    [SerializeField] private Sprite _chargePointer3;
    [SerializeField] private Sprite _chargePointer4;

    private SpriteRenderer _spriteRenderer;
    private PlayerState _playerState;


    public int SpriteNumber { get; set; }
    public bool CanShoot { get; set; }

    #nullable enable
    public GameObject? Target { get; set; }
    #nullable disable


    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerState = transform.root.gameObject.GetComponent<LocalPlayerReference>().PlayerState;
        _playerState.PointerScript = this;
    }


    void Update()
    {
        transform.position = _playerState.MousePosition;

        switch (SpriteNumber)
        {
            case 0:
                _spriteRenderer.sprite = _pointerBase;
                break;
            case 1:
                ChangePointer(_pointer1, _pointer2, _pointer3, _pointer4);
                break;
            case 2:
                ChangePointer(_smartPointer1, _smartPointer2, _smartPointer3, _smartPointer4);
                break;
            case 3:
                ChangePointer(_chargePointer1, _chargePointer2, _chargePointer3, _chargePointer4);
                break;
        }
    }


    private void ChangePointer(Sprite pointer1, Sprite pointer2, Sprite pointer3, Sprite pointer4)
    {
        if (!_playerState.Walking)
        {
            if (CanShoot)
                _spriteRenderer.sprite = pointer1;
            else
                _spriteRenderer.sprite = pointer2;
        }
        else
        {
            if (CanShoot)
                _spriteRenderer.sprite = pointer3;
            else
                _spriteRenderer.sprite = pointer4;
        }
    }


    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ennemy"))
        {
            Target = collision.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ennemy"))
        {
            Target = null;
        }
    }
}