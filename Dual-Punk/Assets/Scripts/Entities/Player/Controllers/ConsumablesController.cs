using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;


public class ConsumablesController : MonoBehaviour
{
    [SerializeField] private GameObject _grenade;
    [SerializeField] private GameObject _grenadePath;

    [SerializeField] private float _throwForce;
    [SerializeField] private float _throwDistance;
    [SerializeField] private float _chargeTime;
    [SerializeField] private float _grenadeTimer;
    [SerializeField] private float _grenadeCurveFactor;
    [SerializeField] private int _lineResolution;

    private LineRenderer _lineRenderer;
    private SpriteRenderer _grenadeImpact;
    private PlayerState _playerState;
    private IDamageable _damageable;

    private Vector3 _direction;
    private Vector3 _offset;


    // Heal
    [SerializeField]
    private float _healCooldown;
    private float _healTimer;

    public float HealCoolDown { get => _healCooldown; }
    public float HealTimer { get => _healTimer; }


    // CombatUseableItem
    [SerializeField]
    private float _itemCooldown;
    private float _itemTimer;
    private float _explodeTimer;
    private bool _chargeGrenade;
    
    public float ItemCoolDown { get => _itemCooldown; }
    public float ItemTimer { get => _itemTimer; }


    void Start()
    {
        _healTimer = _healCooldown;
        _itemTimer = _itemCooldown;
        _lineResolution += 2;

        _offset = new Vector3(0, 0.5f, 0);

        _damageable = GetComponent<IDamageable>();
        _playerState = GetComponent<PlayerState>();
        _lineRenderer = _grenadePath.GetComponentInChildren<LineRenderer>();
        _grenadeImpact = _grenadePath.GetComponentInChildren<SpriteRenderer>();

        _lineRenderer.positionCount = _lineResolution;
        Reset();
    }


    void Update()
    {
        _direction = _playerState.MousePosition - transform.position - _offset;

        if (_healTimer >= _healCooldown)
        {
            if (Input.GetButtonDown("UseHeal") && _playerState.Health < _playerState.MaxHealth)
            {
                _healTimer = 0;
                _damageable.Heal(30);
            }
        }
        else
        {
            _healTimer += Time.deltaTime;
        }
        

        if (_itemTimer >= _itemCooldown)
        {
            if (Input.GetButtonDown("UseGrenade"))
            {
                _chargeGrenade = true;
                _lineRenderer.enabled = true;
                _grenadeImpact.enabled = true;
            }

            if (_chargeGrenade)
            {
                _explodeTimer += Time.deltaTime;

                if (_explodeTimer > _grenadeTimer)
                    Reset();

                if (Input.GetButtonUp("UseGrenade"))
                {
                    GameObject grenade = Instantiate(_grenade, transform.position + _offset, transform.rotation);
                    grenade.GetComponent<GrenadeScript>().Setup(transform.position + _offset, _direction.normalized,
                        _throwForce * Methods.GetDirectionFactor(_direction) * GetChargeFactor(), _grenadeTimer - _explodeTimer,
                        GetThrowDistance(), _grenadeCurveFactor);

                    Reset();
                    _itemTimer = 0;
                }

                DrawGrenadePath();
            }
        }
        else {
            _itemTimer += Time.deltaTime;
        }
    }


    private void DrawGrenadePath()
    {
        _lineRenderer.positionCount = _lineResolution;

        Vector2 direction = _direction.normalized;
        Vector2 startPoint = transform.position + _offset;
        Vector2 endPoint = startPoint + direction * GetChargeFactor() * GetThrowDistance();
        Vector2 normalVector = Vector2.Perpendicular(direction) * GetChargeFactor() * (Methods.GetDirectionFactor(direction) - 0.5f);

        if (_playerState.MousePosition.x < transform.position.x)
            normalVector = -normalVector;

        for (int i = 1; i < _lineResolution - 1; i++)
        {
            float factor = (float)i / (float)(_lineResolution - 1);
            _lineRenderer.SetPosition(i, Vector2.Lerp(startPoint, endPoint, factor) +
                normalVector * (- factor * factor * _grenadeCurveFactor + factor * _grenadeCurveFactor));
        }

        _lineRenderer.SetPosition(0, startPoint);
        _lineRenderer.SetPosition(_lineResolution - 1, endPoint);
        _grenadeImpact.transform.position = endPoint;
    }


    private float GetChargeFactor()
    {
        if (_explodeTimer > _chargeTime)
            return 1;
        return _explodeTimer/_chargeTime;
    }


    private float GetThrowDistance()
    {
        float distance = _throwDistance * GetChargeFactor() * Methods.GetDirectionFactor(_direction);
        if (_direction.magnitude > distance)
            return distance;
        return _direction.magnitude;
    }


    private void Reset()
    {
        _explodeTimer = 0;
        _chargeGrenade = false;
        _lineRenderer.enabled = false;
        _grenadeImpact.enabled = false;
    }
}