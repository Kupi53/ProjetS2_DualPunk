using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class HealthManager : MonoBehaviour, IDamageable
{
    private PlayerState _playerState;

    //Nécessaire pour implant ThermicExchange
    public float DamageMultiplier = 1;
    
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


    private IEnumerator HealthCoroutine(float amount, float time)
    {
        int startHealth = _playerState.Health;
        float timer = 0;
        float healPerTime = amount / time;

        while (timer <= time)
        {
            timer += Time.deltaTime;
            SetHealth(startHealth + (int)(healPerTime * timer));

            yield return null;
        }

        SetHealth(startHealth + (int)amount);
    }


    public void Destroy()
    {
        _playerState.Health = 0;
        CheckHealth();
    }

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

    public void Damage(int amount, float time)
    {
        if (LaserGunScript != null)
        {
            if (LaserGunScript.CoolDownLevel < 1 * (1 - DamageMultiplier))
            {
                LaserGunScript.CoolDownLevel -= 1 * (1 - DamageMultiplier);
            }
        }

        float newAmout = amount * DamageMultiplier;

        if (time == 0)
        {
            _playerState.Health -= (int)newAmout;
            CheckHealth();
        }
        else
        {
            StartCoroutine(HealthCoroutine(-amount, time));
        }
    }

    public void SetHealth(int amount)
    {
        _playerState.Health = amount;
        CheckHealth();
    }
}
