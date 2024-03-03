using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;


//Ce script va gerer les attaques, des implants ou des armes

public class AttacksController : NetworkBehaviour
{
    private PlayerState _playerState;
    private Vector3 _direction;

#nullable enable
    private GameObject? _weapon;
    private WeaponScript? _weaponScript;
#nullable disable


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
                _direction = (_playerState.Pointer.transform.position - transform.position - _weaponScript.WeaponOffset).normalized;

                if (Input.GetButtonDown("Drop"))
                {
                    _weaponScript.Reset();
                    _weaponScript.InHand = false;
                    _playerState.HoldingWeapon = false;
                    _weaponScript.PointerScript.Locked = false;
                    _weaponScript.PointerScript.SpriteNumber = 0;
                }
            }

            if (_playerState.HoldingWeapon)
                _weaponScript.Run(transform.position, _direction);
        }
    }
}