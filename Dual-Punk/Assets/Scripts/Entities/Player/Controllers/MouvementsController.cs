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

    private List<(Vector2, float)> _forces;
    private PlayerState _playerState;
    private Rigidbody2D _rb2d;

    private Vector2 _moveDirection;
    private Vector2 _pointerDirection;

    private bool _enableMovement;
    private float _dashTimer;
    private float _dashCooldownTimer;
    private float _moveSpeed;
    private float _moveFactor;


    private void Start()
    {
        _dashTimer = 0;
        _dashCooldownTimer = 0;
        _enableMovement = true;

        _rb2d = GetComponent<Rigidbody2D>();
        _playerState = GetComponent<PlayerState>();
        _forces = new List<(Vector2, float)>();
    }


    // Prends Imputs chaque frame
    private void Update()
    {
        if (!IsOwner) return;

        if (_enableMovement)
        {
            _moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") * 0.5f).normalized;
            _pointerDirection = (_playerState.MousePosition - transform.position).normalized;

            if (_moveDirection == Vector2.zero) {
                _playerState.Moving = false;
            }
            else {
                _playerState.Moving = true;
            }

            if (_playerState.Down)
            {
                _moveSpeed = _crawlSpeed;
                _playerState.CrawlTimer += Time.deltaTime;
                if (_playerState.CrawlTimer > _playerState.CrawlTime)
                {
                    _enableMovement = false;
                    _moveDirection = Vector2.zero;
                }
            }
            else if (Input.GetButton("Walk") || _moveDirection == Vector2.zero) {
                _playerState.Walking = true;
                _moveSpeed = _walkSpeed;
            }
            else {
                _playerState.Walking = false;
                _moveSpeed = _sprintSpeed;
            }

            if (Input.GetButtonDown("Dash") && !_playerState.Down && _dashCooldownTimer <= 0 && _playerState.Moving)
            {
                _enableMovement = false;
                _playerState.Dashing = true;
                _dashCooldownTimer = _dashCooldown;
            }
        }
        else if (!_playerState.Down && !_playerState.Dashing)
        {
            _enableMovement = true;
            _playerState.CrawlTimer = 0;
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

        if (_enableMovement)
        {
            float pointerAngle = Methods.GetAngle(_pointerDirection);

            _moveFactor = 1;

            if (_playerState.Moving)
            {
                float moveAngle = Methods.GetAngle(_moveDirection);

                _moveFactor *= Methods.GetDirectionFactor(_moveDirection);

                if (!_playerState.HoldingWeapon) {
                    _playerState.AnimAngle = moveAngle;
                }
                else if (!Methods.SameDirection(moveAngle, pointerAngle, 60)) {
                    _moveFactor *= _moveBackFactor;
                }
            }

            if (_playerState.HoldingWeapon)
                _playerState.AnimAngle = pointerAngle;
        }

        else if (_playerState.Dashing)
        {
            if (_dashTimer < _dashTime)
            {
                _moveSpeed = _dashSpeed - _dashTimer;
                _dashTimer += Time.fixedDeltaTime;
            }
            else
            {
                _dashTimer = 0;
                _enableMovement = true;
                _playerState.Dashing = false;
            }
        }

        MovePosition();
    }


    private void MovePosition()
    {
        int i = 0;
        Vector2 resultingForce = Vector2.zero;

        while (i < _forces.Count)
        {
            if (_forces[i].Item2 <= 0)
            {
                _forces.Remove(_forces[i]);
            }
            else
            {
                resultingForce += _forces[i].Item1 * _forces[i].Item2;
                _forces[i] = (_forces[i].Item1, _forces[i].Item2 - Time.deltaTime * _playerState.ForcesDecreaseSpeed);
                i++;
            }
        }

        _rb2d.MovePosition(_rb2d.position + resultingForce * Methods.GetDirectionFactor(resultingForce) * _playerState.ForcesEffect + _moveDirection * _moveSpeed * _moveFactor);
    }


    public void Impact(Vector2 direction, float intensity)
    {
        _forces.Add((direction.normalized, intensity));
    }
}