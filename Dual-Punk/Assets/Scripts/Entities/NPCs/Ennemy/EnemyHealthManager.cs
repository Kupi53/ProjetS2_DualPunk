using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(EnemyState))]
public class EnemyHealthManager : MonoBehaviour, IDamageable
{
    [SerializeField] private int[] _lives;
    [SerializeField] private float _imuneTime;

    private EnemyState _enemyState;
    private EnemyHealthIndicator _healthIndicator;
    private float _imunityTimer;
    private int _maxHealth;

    public int Index { get; set; }
    public int[] Lives { get => _lives; }


    private void Start()
    {
        Index = 0;
        _imunityTimer = -1;
        _maxHealth = _lives[0];
        _enemyState = GetComponent<EnemyState>();
        _healthIndicator = GetComponent<EnemyHealthIndicator>();
    }


    private void Update()
    {
        if (_imunityTimer > 0)
        {
            _imunityTimer -= Time.deltaTime;
            _enemyState.Stop = true;
        }
        else if (_imunityTimer > -1)
        {
            _imunityTimer = -1;
            _enemyState.Stop = false;
        }
    }


    private IEnumerator HealthCoroutine(int amount, float time)
    {
        int newAmount;
        int lastAmount = 0;
        float timer = 0;
        float healPerTime = ((float)amount) / time;

        while (timer <= time)
        {
            timer += Time.deltaTime;
            newAmount = (int)(healPerTime * timer);
            SetHealth(_lives[Index] - lastAmount + newAmount);
            lastAmount = newAmount;

            yield return null;
        }
    }


    private void CheckHealth()
    {
        if (_lives[Index] > _maxHealth)
            _lives[Index] = _maxHealth;

        else if (_lives[Index] <= 0)
        {
            Index++;
            if (Index == _lives.Length)
            {
                DestroyObject();
            }
            else
            {
                _maxHealth = _lives[Index];
                _imunityTimer = _imuneTime;
                GetComponent<EnemyWeaponHandler>().AssignWeapon();
            }
        }
    }


    public bool DestroyObject()
    {
        GetComponent<EnemyWeaponHandler>().DropWeapon();
        Destroy(gameObject);
        return true;
    }


    public void Heal(int amount, float time)
    {
        if (_imunityTimer > 0) return;

        if (time == 0)
        {
            _lives[Index] += amount;
            CheckHealth();
        }
        else
        {
            StartCoroutine(HealthCoroutine(amount, time));
        }
    }

    public void Damage(int amount, float time, bool warriorLuckBullet)
    {
        if (_imunityTimer > 0) return;

        _healthIndicator.DisplayDamageIndicator(amount, warriorLuckBullet);

        if (time == 0)
        {
            _lives[Index] -= amount;
            CheckHealth();
        }
        else
        {
            StartCoroutine(HealthCoroutine(-amount, time));
        }
    }

    public void SetHealth(int amount)
    {
        if (_imunityTimer > 0) return;

        _lives[Index] = amount;
        CheckHealth();
    }
}