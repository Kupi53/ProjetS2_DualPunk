using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class HealthManager : NetworkBehaviour, IDamageable
{
    private PlayerState _playerState;

    //Nécessaire pour implant ThermicExchange
    public float DamageMultiplier;
    
#nullable enable
    public LaserGunScript? LaserGunScript;
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

        SetHealth(_playerState.Health - lastAmount + amount);
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
    public void Damage(int amount, float time)
    {
        float newAmout = amount;

        if (LaserGunScript != null)
        {
            float coolDownlevel = 1 * (1 - DamageMultiplier);
            Debug.Log(coolDownlevel);
            if (LaserGunScript.CoolDownLevel > coolDownlevel)
            {
                LaserGunScript.CoolDownLevel -= coolDownlevel;
            }

            newAmout *= DamageMultiplier;
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