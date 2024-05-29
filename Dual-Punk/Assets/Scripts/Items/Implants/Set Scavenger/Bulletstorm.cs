using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using FishNet.Object;

public class Bulletstorm : ImplantScript
{
    private CircleCollider2D _shield;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    
    void Awake()
    {
        Type = ImplantType.ExoSqueleton;
        SetName = "Scavenger";

        _shield = gameObject.GetComponent<CircleCollider2D>();
        _animator = gameObject.GetComponent<Animator>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public override void Run()
    {
        if (IsEquipped && Input.GetButton("SecondaryUse"))
        {
            gameObject.tag = "Wall";
            gameObject.layer = 8;
            _shield.enabled = true;
            _animator.enabled = true;
            _spriteRenderer.enabled = true;
            _animator.Play("Shield");
            PlayerState.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            
            PlayerState.GetComponent<MouvementsController>().enabled = false;
            PlayerState.GetComponent<AttacksController>().enabled = false;
        }
        else
        {
            gameObject.tag = "Implant";
            gameObject.layer = 7;
            _shield.enabled = false;
            _animator.enabled = false;
            _spriteRenderer.enabled = false;
            PlayerState.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            
            PlayerState.GetComponent<MouvementsController>().enabled = true;
            PlayerState.GetComponent<AttacksController>().enabled = true;
        }
    }

    public override void ResetImplant()
    {
        gameObject.tag = "Implant";
        gameObject.layer = 7;
        _shield.enabled = false;
        _animator.enabled = false;
        _spriteRenderer.enabled = false;
        PlayerState.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            
        PlayerState.GetComponent<MouvementsController>().enabled = true;
        PlayerState.GetComponent<AttacksController>().enabled = true;
        
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}