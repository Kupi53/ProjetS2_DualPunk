using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Meme but mais different de PlayerState, ici on donne les stats des capacites du joueur non de son etat
public class ConsumablesController : MonoBehaviour
{
    [SerializeField] private GameObject _grenade;

    [SerializeField] private float _throwForce;
    [SerializeField] private float _throwDistance;
    [SerializeField] private float _chargeTime;
    // temps avant explosion de la grenade apres armement
    [SerializeField] private float _grenadeTimer;

    private LineRenderer _lineRenderer;
    private PlayerState _playerState;
    private IDamageable _damageable;
    private Vector3 _direction;


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
    // timer pour le temps avant explosion de la grenade
    private float _explodeTimer;
    private bool _chargeGrenade;
    
    public float ItemCoolDown { get => _itemCooldown; }
    public float ItemTimer { get => _itemTimer; }


    void Start()
    {
        _explodeTimer = 0;
        _healTimer = _healCooldown;
        _itemTimer = _itemCooldown;
        _damageable = GetComponent<IDamageable>();
        _playerState = GetComponent<PlayerState>();
        _lineRenderer = GetComponentInChildren<LineRenderer>();
    }


    void Update()
    {
        _direction = _playerState.MousePosition - transform.position;

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
            }
            if (_chargeGrenade)
            {
                _explodeTimer += Time.deltaTime;

                if (_explodeTimer > _grenadeTimer)
                {
                    Reset();
                }
                if (Input.GetButtonUp("UseGrenade"))
                {
                    GameObject grenade = Instantiate(_grenade, transform.position, transform.rotation);
                    grenade.GetComponent<GrenadeScript>().Setup(transform.position, _direction.normalized,
                        _throwForce * GetChargeFactor(), _grenadeTimer - _explodeTimer, GetThrowDistance());

                    Reset();
                    _itemTimer = 0;
                }
            }
        }
        else
        {
            _itemTimer += Time.deltaTime;
        }
    }


    private float GetChargeFactor()
    {
        if (_explodeTimer > _chargeTime)
            return 1;
        return _chargeTime - _explodeTimer;
    }


    private float GetThrowDistance()
    {
        if (_throwDistance * GetChargeFactor() < _direction.magnitude)
            return _throwDistance * GetChargeFactor();
        return _direction.magnitude;
    }


    private void Reset()
    {
        _explodeTimer = 0;
        _chargeGrenade = false;
    }
}
