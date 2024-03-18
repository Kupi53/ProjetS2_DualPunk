using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using Unity.Netcode;
using TMPro;
using System.Data.SqlTypes;


public class MouvementsController : NetworkBehaviour
{
    private Rigidbody2D _rb2d;
    private Animator _animator;
    private PlayerState _playerState;

    // Constantes qui sont les noms des sprites du joueur
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

    // Nombres decimaux pour gerer la vitesse de marche, course et de dash
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _sprintSpeed;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashTime;
    [SerializeField] private float _moveBackFactor;

    // Bool qui sert pour le state du joueur (par exemple pendant un dash ou certaines abilitiés le joueur ne doit pas pouvoir bouger)
    private bool _enableMovement;
    private float _dashTimer;
    private string _currentState;

    // Vitesse de deplacement en fonction des variables publiques en dessous
    private float _moveSpeed;
    // Facteur qui depend aussi de certaines variables en dessous qui ralenti ou accelere le deplacement dans certaines situations
    private float _moveFactor;

    private Vector2 _moveDirection;
    private Vector2 _pointerDirection;


    // Start is called before the first frame update
    void Start()
    {
        _dashTimer = 0;
        _enableMovement = true;
        _currentState = PLAYER_IDLE;

        _rb2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _playerState = GetComponent<PlayerState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        // Prends Imputs chaque frame
        if (_enableMovement)
        {
            _moveFactor = 1;
            if (_playerState.Walking)
                _moveSpeed = _walkSpeed;
            else
                _moveSpeed = _sprintSpeed;

            // Direction du deplacement
            _moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") * 0.5f).normalized;

            // Direction du pointeur
            if (_playerState.PointerScript != null)
                _pointerDirection = (_playerState.MousePosition - transform.position).normalized;
            
            if (Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0)
                _moveFactor *= 0.75f;
            else if (Input.GetAxis("Horizontal") == 0)
                _moveDirection *= 0.5f;

            if (Input.GetButton("Walk") || _moveDirection == Vector2.zero)
                _playerState.Walking = true;
            else
                _playerState.Walking = false;
        }

        if (Input.GetButtonDown("Dash") && _playerState.DashCooldown <= 0.0f)
        {
            _enableMovement = false;
            _playerState.Dashing = true;
            _playerState.DashCooldown = _playerState.DashCooldownMax;
        }
        else if (_playerState.DashCooldown > 0.0f)
        {
            _playerState.DashCooldown -= Time.deltaTime;
        }
    }


    void FixedUpdate()
    {
        if (!IsOwner) return;

        if (_enableMovement)
        {
            float moveAngle = (float)(Math.Atan2(_moveDirection.y, _moveDirection.x) * (180 / Math.PI));
            float pointerAngle = (float)(Math.Atan2(_pointerDirection.y, _pointerDirection.x) * (180 / Math.PI));

            if (_playerState.HoldingWeapon && !Methods.SameDirection(moveAngle, pointerAngle, 60))
                _moveFactor *= _moveBackFactor;

            if (_moveDirection != new Vector2(0, 0))
            {
                _rb2d.MovePosition(_rb2d.position + _moveDirection * _moveSpeed * _moveFactor);

                if (!_playerState.HoldingWeapon)
                    ChangeAnimation(ChangeDirection(moveAngle));
            }

            else if (!_playerState.HoldingWeapon)
                _animator.Play(PLAYER_IDLE);

            if (_playerState.HoldingWeapon)
                ChangeAnimation(ChangeDirection(pointerAngle));
        }

        else if (_playerState.Dashing)
        {
            if (_dashTimer < _dashTime)
            {
                _rb2d.MovePosition(_rb2d.position + _moveDirection * (_dashSpeed - _dashTimer));
                _dashTimer += Time.fixedDeltaTime;
            }
            else
            {
                _enableMovement = true;
                _playerState.Dashing = false;
                _dashTimer = 0;
            }
        }
    }


    // Utilise dans anim mouvement, change l'animation en fonction des constantes Player_S, Player_...
    void ChangeAnimation(string newState)
    {
        if (_currentState == newState) return;

        _animator.Play(newState);
        _currentState = newState;
    }

    // On passe la direction actuelle du joueur et en fonction, appelle changeAnimation 
    // Avec la constante (nom du sprite) adaptée
    string ChangeDirection(float angle)
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
        else if ((angle > 157 &&  angle <= 180) || (angle >= -180 && angle <= -158))
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
    
    string ChangeDirectionWeapon(float angle)
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
        else if ((angle > 157 &&  angle <= 180) || (angle >= -180 && angle <= -158))
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