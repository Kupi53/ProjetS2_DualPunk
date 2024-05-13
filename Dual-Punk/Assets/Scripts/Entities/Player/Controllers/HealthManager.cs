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
        int lastAmount = 0;
        int newAmount = 0;
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
