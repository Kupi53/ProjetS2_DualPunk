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


    void Start()
    {
        _currentState = PLAYER_IDLE;
        _playerState = GetComponent<PlayerState>();
        _networkAnimator = GetComponent<NetworkAnimator>();
    }


    private void Update()
    {
        if (!IsOwner) return;
        if (_playerState.Moving || _playerState.HoldingWeapon)
            ChangeAnimation(SelectAnimation(_playerState.AnimAngle));
        else
            _networkAnimator.Play(PLAYER_IDLE);
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
}
