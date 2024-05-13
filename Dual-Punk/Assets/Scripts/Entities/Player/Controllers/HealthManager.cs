using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class HealthManager : MonoBehaviour, IDamageable
{
    private PlayerState _playerState;

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
        int startHealth = _playerState.Health;
        float healPerTime = ((float)amount) / time;
        float timer = 0;

        while (timer <= time)
        {
            timer += Time.deltaTime;
            SetHealth(startHealth + (int)(healPerTime * timer));

            yield return null;
        }

        SetHealth(startHealth + amount);
    }


    public bool Destroy()
    {
        _playerState.Health = 0;
        CheckHealth();
        return true;
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
        if (time == 0)
        {
            _playerState.Health -= amount;
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
