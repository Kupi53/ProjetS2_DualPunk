using System.Collections;
using System.Collections.Generic;
using FishNet.Component.Animating;
using FishNet.Object;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    private PlayerState _playerState;
    private NetworkAnimator _networkAnimator;
    private string _currentState;

    const string PLAYER_N = "Player N";
    const string PLAYER_E = "Player E";
    const string PLAYER_S = "Player S";
    const string PLAYER_W = "Player W";
    const string PLAYER_NE = "Player NE";
    const string PLAYER_NW = "Player NW";
    const string PLAYER_SE = "Player SE";
    const string PLAYER_SW = "Player SW";
    const string PLAYER_IDLE = "Player Idle";

    const string PLAYERWEAPON_N = "PlayerWeapon N";
    const string PLAYERWEAPON_E = "PlayerWeapon E";
    const string PLAYERWEAPON_S = "PlayerWeapon S";
    const string PLAYERWEAPON_W = "PlayerWeapon W";
    const string PLAYERWEAPON_NE = "PlayerWeapon NE";
    const string PLAYERWEAPON_NW = "PlayerWeapon NW";
    const string PLAYERWEAPON_SE = "PlayerWeapon SE";
    const string PLAYERWEAPON_SW = "PlayerWeapon SW";


    void Start()
    {
        _currentState = PLAYER_IDLE;
        _playerState = GetComponent<PlayerState>();
        _networkAnimator = GetComponent<NetworkAnimator>();
    }


    private void Update()
    {
        if (!IsOwner) return;
        if (!_playerState.Moving && _playerState.HoldingWeapon)
            ChangeAnimation(SelectAnimationWeapon(_playerState.AnimAngle));
        else if (_playerState.Moving)
            ChangeAnimation(SelectAnimation(_playerState.AnimAngle));
        else
            ChangeAnimation(PLAYER_IDLE);
    }


    // Utilise dans anim mouvement, change l'animation en fonction des constantes Player_S, Player_N...
    private void ChangeAnimation(string newState)
    {
        if (_currentState != newState)
        {
            _currentState = newState;
            _networkAnimator.Play(newState);
        }
    }


    // On passe la direction actuelle du joueur et en fonction, appelle changeAnimation avec la constante (nom du sprite) adapt�e
    private string SelectAnimation(float angle)
    {
        if (angle > -22 && angle <= 22)
        {
            return PLAYER_E;
        }
        else if (angle > 22 && angle <= 67)
        {
            return PLAYER_NE;
        }
        else if (angle > 67 && angle <= 112)
        {
            return PLAYER_N;
        }
        else if (angle > 112 && angle <= 157)
        {
            return PLAYER_NW;
        }
        else if ((angle > 157 && angle <= 180) || (angle >= -180 && angle <= -158))
        {
            return PLAYER_W;
        }
        else if (angle > -158 && angle <= -113)
        {
            return PLAYER_SW;
        }
        else if (angle > -113 && angle <= -68)
        {
            return PLAYER_S;
        }
        else
        {
            return PLAYER_SE;
        }
    }

    private string SelectAnimationWeapon(float angle)
    {
        if (angle > -22 && angle <= 22)
        {
            return PLAYERWEAPON_E;
        }
        else if (angle > 22 && angle <= 67)
        {
            return PLAYERWEAPON_NE;
        }
        else if (angle > 67 && angle <= 112)
        {
            return PLAYERWEAPON_N;
        }
        else if (angle > 112 && angle <= 157)
        {
            return PLAYERWEAPON_NW;
        }
        else if ((angle > 157 && angle <= 180) || (angle >= -180 && angle <= -158))
        {
            return PLAYERWEAPON_W;
        }
        else if (angle > -158 && angle <= -113)
        {
            return PLAYERWEAPON_SW;
        }
        else if (angle > -113 && angle <= -68)
        {
            return PLAYERWEAPON_S;
        }
        else
        {
            return PLAYERWEAPON_SE;
        }
    }
}
