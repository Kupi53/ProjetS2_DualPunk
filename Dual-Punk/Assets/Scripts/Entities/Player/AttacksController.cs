using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;


//Ce script va gerer les attaques, des implants ou des armes

public class AttacksController : NetworkBehaviour
{
    private Vector3 _direction;
    private Vector3 _pointerPosition;
    private PlayerState _playerState;


    private void Start()
    {
        _playerState = GetComponent<PlayerState>();
    }

    private void Update()
    {
        if (_playerState.HoldingWeapon)
        {
            if (!_playerState.Attacking)
            {
                _pointerPosition = 
        
                _direction = (_playerState.MousePosition - transform.position - _playerState.WeaponScript.WeaponOffset).normalized;

                if (Input.GetButtonDown("Drop"))
                {
                    _playerState.WeaponScript.Reset();
                    _playerState.WeaponScript.InHand = false;
                    _playerState.HoldingWeapon = false;
                    _playerState.PointerScript.SpriteNumber = 0;
                }
            }

            if (_playerState.HoldingWeapon)
                _playerState.WeaponScript.Run(transform.position, _direction);
        }
    }
}