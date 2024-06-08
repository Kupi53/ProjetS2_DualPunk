using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using FishNet.Object;
using TMPro;
using System.Data.SqlTypes;


public class MouvementsController : NetworkBehaviour, IImpact
{
    // Nombres decimaux pour controller la vitesse de marche, course et de dash
    [SerializeField] private float _crawlSpeed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _sprintSpeed;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashTime;
    [SerializeField] private float _dashCooldown;
    [SerializeField] private float _moveBackFactor;
    [SerializeField] private float _forcesDecreaseSpeed;

    private List<(Vector2, float)> _forces;
    private PlayerState _playerState;
    private Rigidbody2D _rb2d;
    private Vector2 _moveDirection;

    private float _dashTimer;
    private float _dashCooldownTimer;
    private float _moveSpeed;
    private float _moveFactor;
    
    // Pour implant TeleportStrike
    public bool EnableDash { get; set; } = true;

    public bool EnableMovement { get; set; }
    public float WalkSpeed { get =>  _walkSpeed; set => _walkSpeed = value; }
    public float SprintSpeed { get => _sprintSpeed; set => _sprintSpeed = value; }


    private void Start()
    {
        _dashTimer = 0;
        _dashCooldownTimer = 0;
        _moveSpeed = _sprintSpeed;

        _moveDirection = Vector2.zero;
        _forces = new List<(Vector2, float)>();

        _rb2d = GetComponent<Rigidbody2D>();
        _playerState = GetComponent<PlayerState>();
    }


    // Prends Imputs chaque frame
    private void Update()
    {
        if (!IsOwner) return;

        if (EnableMovement)
        {
            _moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") * 0.5f).normalized;
            if (_moveDirection == Vector2.zero)
                _playerState.Moving = false;
            else
                _playerState.Moving = true;

            if (_playerState.IsDown)
            {
                _moveSpeed = _crawlSpeed;
                _playerState.CrawlTimer += Time.deltaTime;

                if (_playerState.CrawlTimer > _playerState.CrawlTime)
                {
                    EnableMovement = false;
                }
            }
            else if (!_playerState.Stop)
            {
                if (Input.GetButtonDown("Walk"))
                    _playerState.Walking = true;
                else if (Input.GetButtonUp("Walk"))
                    _playerState.Walking = false;

                if (Input.GetButtonDown("Dash") && _dashCooldownTimer <= 0 && _playerState.Moving && EnableDash)
                {
                    EnableMovement = false;
                    _playerState.Dashing = true;
                    _dashCooldownTimer = _dashCooldown;
                }
                else
                {
                    _moveSpeed = _playerState.Walking ? _walkSpeed : _sprintSpeed;
                }
            }
        }
        else
        {
            if (!_playerState.IsDown && !_playerState.Dashing && (PromptManager.Instance.CurrentPromptShown == null
                || PromptManager.Instance.CurrentPromptShown.GetComponent<PromptController>().Prompt.EnableMovement))
            {
                EnableMovement = true;
                _playerState.CrawlTimer = 0;
            }

            _playerState.Walking = false;
        }

        if (_dashCooldownTimer > 0)
        {
            _dashCooldownTimer -= Time.deltaTime;
        }
    }


    // Les deplacements avec un rb2d sont dans FixedUpdate
    private void FixedUpdate()
    {
        if (!IsOwner) return;

        _moveFactor = 0;

        if (EnableMovement && !_playerState.Stop && _playerState.Moving)
        {
            float moveAngle = Methods.GetAngle(_moveDirection);
            _moveFactor = Methods.GetDirectionFactor(_moveDirection);

            if (!_playerState.HoldingWeapon)
            {
                _playerState.AnimAngle = moveAngle;
            }
            else if (!Methods.SameDirection(moveAngle, _playerState.AnimAngle, 60))
            {
                _moveFactor *= _moveBackFactor;
            }            
        }

        else if (_playerState.Dashing)
        {
            if (_dashTimer < _dashTime)
            {
                _moveSpeed = _dashSpeed - _dashTimer;
                _moveFactor = Methods.GetDirectionFactor(_moveDirection);
                _dashTimer += Time.fixedDeltaTime;
            }
            else
            {
                _dashTimer = 0;
                EnableMovement = true;
                _playerState.Dashing = false;
            }
        }

        else
        {
            _playerState.Moving = false;
        }

        MovePosition();
    }


    private void MovePosition()
    {
        int i = 0;
        Vector2 resultingForce = Vector2.zero;
        Vector2 targetDirection = Vector2.zero;

        if (_playerState.Moving)
        {
            targetDirection = _moveDirection * _moveSpeed * _moveFactor;
        }

        if (_forces.Count > 0)
        {
            while (i < _forces.Count)
            {
                if (_forces[i].Item2 <= 0)
                {
                    _forces.Remove(_forces[i]);
                }
                else
                {
                    resultingForce += _forces[i].Item1 * _forces[i].Item2;
                    _forces[i] = (_forces[i].Item1, _forces[i].Item2 - Time.deltaTime * _forcesDecreaseSpeed);
                    i++;
                }
            }

            resultingForce *= Methods.GetDirectionFactor(resultingForce) * _playerState.ForcesEffect;
        }

        _rb2d.MovePosition(_rb2d.position + targetDirection + resultingForce);
    }


    public void SetDash(float dashSpeedMultiplier, float dashCooldownMultiplier)
    {
        _dashSpeed *= dashSpeedMultiplier;
        _dashTime *= dashSpeedMultiplier;
        _dashCooldown *= dashCooldownMultiplier;
    }


    // Les forces ajoutees viennent tjrs d'un server rpc, du coup ca marchait pas sur le client
    [ObserversRpc]
    public void Impact(Vector2 direction, float intensity)
    {
        _forces.Add((direction.normalized, intensity));
    }
}