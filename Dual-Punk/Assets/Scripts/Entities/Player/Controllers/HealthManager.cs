using System;
using System.Collections;
using System.Collections.Generic;
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


    public void Heal(float amount)
    {
        if (_playerState.Health + amount > _playerState.MaxHealth)
        {
            _playerState.Health = _playerState.MaxHealth;
        }
        else
            _playerState.Health += amount;
    }


    public void Damage(float amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException("Cannot have Negative damage.");
        }
        _playerState.Health -= amount;

        if (_playerState.Health <= 0)
        {
            Die();
        }
    }
}
