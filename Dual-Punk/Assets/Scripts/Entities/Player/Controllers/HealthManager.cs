using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class HealthManager : NetworkBehaviour, IDamageable
{
    [SerializeField] private float _damageToSecondConversionFactor;

    private PlayerState _playerState;

    //Nécessaire pour implant ThermicExchange
    public float DamageMultiplier { get; set; }
#nullable enable
    public LaserGunScript? LaserGunScript { get; set; }
#nullable disable


    private void Start()
    {
        _playerState = GetComponent<PlayerState>();
    }


    private void CheckHealth()
    {
        if (_playerState.Health > _playerState.MaxHealth)
            _playerState.Health = _playerState.MaxHealth;

        else if (_playerState.Health <= 0)
        {
            _playerState.Health = 0;
            _playerState.Down = true;
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
            SetHealth(_playerState.Health - lastAmount + newAmount);
            lastAmount = newAmount;

            yield return null;
        }
    }


    public bool DestroyObject()
    {
        _playerState.Health = 0;
        CheckHealth();
        return true;
    }

    [ObserversRpc]
    public void Heal(int amount, float time)
    {
        if (time == 0)
        {
            _playerState.Health += amount;
            CheckHealth();
        }
        else
        {
            StartCoroutine(HealthCoroutine(amount, time));
        }
    }

    [ObserversRpc]
    public void Damage(int amount, float time, bool crit)
    {
        float newAmout = amount;

        if (LaserGunScript != null)
        {
            newAmout *= DamageMultiplier;
            float addedTime = (newAmout / _damageToSecondConversionFactor) / LaserGunScript.FireTime;
            if (LaserGunScript.CoolDownLevel + addedTime > LaserGunScript.FireTime)
            {
                LaserGunScript.CoolDownLevel = LaserGunScript.FireTime;
            }
            else
            {
                LaserGunScript.CoolDownLevel += addedTime;
            }
        }

        if (time == 0)
        {
            _playerState.Health -= (int)newAmout;
            CheckHealth();
        }
        else
        {
            StartCoroutine(HealthCoroutine(-(int)newAmout, time));
        }
    }


    public void SetHealth(int amount)
    {
        _playerState.Health = amount;
        CheckHealth();
    }
}