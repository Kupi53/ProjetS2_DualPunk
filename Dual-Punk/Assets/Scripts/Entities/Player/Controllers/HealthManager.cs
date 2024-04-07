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


    public void Die()
    {
        Debug.Log("You died.");
        Destroy(gameObject);
    }


    private void CheckHealth()
    {
        if (_playerState.Health > _playerState.MaxHealth)
            _playerState.Health = _playerState.MaxHealth;

        else if (_playerState.Health <= 0)
            Die();
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
