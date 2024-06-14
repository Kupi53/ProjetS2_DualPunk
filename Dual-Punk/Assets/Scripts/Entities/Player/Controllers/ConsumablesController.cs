using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using FishNet.Object;
using UnityEngine.UI;
using TMPro;


public class ConsumablesController : NetworkBehaviour
{
    [SerializeField] private GameObject _grenade;
    [SerializeField] private GameObject _grenadePath;
    [SerializeField] private float _throwForce;
    [SerializeField] private float _throwDistance;
    [SerializeField] private float _chargeTime;
    [SerializeField] private float _grenadeTimer;
    [SerializeField] private float _grenadeCurveFactor;
    [SerializeField] private int _lineResolution;
    [SerializeField] private int _healHP;

    private LineRenderer _lineRenderer;
    private SpriteRenderer _grenadeImpact;
    private PlayerState _playerState;
    private IDamageable _damageable;

    private Vector3 _direction;
    private Vector3 _offset;
    private Vector2 _normalVector;


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

        ResetThrow();
    }


    void Update()
    {
        if (!IsOwner) return;

        if (_healTimer >= _healCooldown && !_playerState.IsDown && !_playerState.Stop)
        {
            if (Input.GetButtonDown("UseHeal") && _playerState.Health < _playerState.MaxHealth)
            {
                _healTimer = 0;
                _damageable.Heal(_healHP, 0.2f);

                //enables to display countdown in inventory
                StartCD(_healCooldown, "HealthSyringeCD");

                StartCoroutine(TriggerHealAnimation());
            }
        }
        else {
            _healTimer += Time.deltaTime;
        }
        
        if (_itemTimer >= _itemCooldown && !_playerState.IsDown && !_playerState.Stop)
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
                _direction = _playerState.MousePosition - transform.position - _offset;

                if (_explodeTimer > _grenadeTimer)
                    ResetThrow();

                if (Input.GetButtonUp("UseGrenade"))
                {
                    float throwDistance = GetThrowDistance();

                    ThrowGrenadeRpc(transform.position + _offset, _direction.normalized, _normalVector,
                        _throwForce * GetChargeFactor() * (throwDistance + 0.25f) / _throwDistance,
                        _grenadeTimer - _explodeTimer, throwDistance, _grenadeCurveFactor);
                    
                    ResetThrow();
                    _itemTimer = 0;

                    StartCD(_itemCooldown, "TimerGrenadeCD");
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
        Vector2 direction = _direction.normalized;
        Vector2 startPoint = transform.position + _offset;
        Vector2 endPoint = startPoint + direction * GetChargeFactor() * GetThrowDistance();

        _normalVector = Vector2.Perpendicular(direction) * GetChargeFactor() * (Methods.GetDirectionFactor(direction) - 0.5f);

        if (_playerState.MousePosition.x < transform.position.x)
            _normalVector = -_normalVector;

        for (int i = 1; i < _lineResolution - 1; i++)
        {
            float factor = (float)i / (float)(_lineResolution - 1);
            _lineRenderer.SetPosition(i, Vector2.Lerp(startPoint, endPoint, factor) +
                _normalVector * (- factor * factor * _grenadeCurveFactor + factor * _grenadeCurveFactor));
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

    private void ResetThrow()
    {
        _explodeTimer = 0;
        _chargeGrenade = false;
        _lineRenderer.enabled = false;
        _grenadeImpact.enabled = false;
    }


    [ServerRpc(RequireOwnership = false)]
    private void ThrowGrenadeRpc(Vector3 startPosition, Vector3 moveDirection, Vector3 verticalDirection, float moveSpeed, float explosionTimer, float distanceUntilStop, float curveFactor)
    {
        GameObject grenade = Instantiate(_grenade, startPosition, Quaternion.identity);
        grenade.GetComponent<InstantGrenadeScript>().Setup(startPosition, moveDirection, verticalDirection, moveSpeed, explosionTimer, distanceUntilStop, curveFactor);
        Spawn(grenade);
    }



    private GameObject FindConsummable(string name) {

        CoolDownDisplay[] consummables = Resources.FindObjectsOfTypeAll<CoolDownDisplay>();
        GameObject neededConsummable = null;

        int i = 0;
        while (neededConsummable == null && consummables.Length > i) {
            if (consummables[i].name == name) {
                neededConsummable = consummables[i].gameObject;
            }
            i++;
        }

        return neededConsummable;
    }

    private IEnumerator TriggerCountDown(float coolDownTimer, Text countDownDisplay)
    {
        float currentTimer = coolDownTimer;
        countDownDisplay.text = coolDownTimer.ToString();

        while (currentTimer > 0)
        {
            countDownDisplay.text = currentTimer.ToString();
            yield return new WaitForSeconds(1f);
            currentTimer--;
        }
        countDownDisplay.transform.parent.gameObject.SetActive(false);
    }

    private void StartCD(float coolDownTimer, string name)
    {
        CoolDownDisplay inventoryCDdisplay = FindConsummable(name).GetComponent<CoolDownDisplay>();
        inventoryCDdisplay.gameObject.SetActive(true);
        StartCoroutine(TriggerCountDown(coolDownTimer, inventoryCDdisplay.gameObject.transform.GetChild(0).GetComponent<Text>()));
    }

    private IEnumerator TriggerHealAnimation()
    {
        GameObject healParticleAnimation = transform.GetChild(2).gameObject;
        healParticleAnimation.SetActive(true);
        yield return new WaitForSeconds(1f);
        healParticleAnimation.SetActive(false);
    }

}