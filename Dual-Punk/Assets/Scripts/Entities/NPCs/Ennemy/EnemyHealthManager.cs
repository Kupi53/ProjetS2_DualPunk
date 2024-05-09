using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyHealthManager : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHealth;
    [SerializeField] private float _damageFrequency;
    private int _health;

    private void Start()
    {
        _health = _maxHealth;
    }


    private IEnumerator HealthCoroutine(int amount, float time)
    {
        int startHealth = _health;
        int damageNumber = (int)(time / _damageFrequency);
        int counter = 0;
        float timer = 0;

        while (counter < damageNumber)
        {
            if (timer < _damageFrequency)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0;
                counter++;
                SetHealth(startHealth + amount / damageNumber * counter);
            }

            yield return null;
        }

        SetHealth(startHealth + amount);
    }

    private IEnumerator VisualIndicator(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.15f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }


    private void CheckHealth()
    {
        if (_health > _maxHealth)
            _health = _maxHealth;

        else if (_health <= 0)
        {
            Destroy();
        }
    }


    public void Destroy()
    {
        Destroy(gameObject);
    }


    public void Heal(int amount, float time)
    {
        if (time == 0)
        {
            _health += amount;
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
            _health -= amount;
            CheckHealth();
            StartCoroutine(VisualIndicator(Color.red));
        }
        else
        {
            StartCoroutine(HealthCoroutine(-amount, time));
        }
    }

    public void SetHealth(int amount)
    {
        if (amount < _health)
        {
            StartCoroutine(VisualIndicator(Color.black));
        }

        _health = amount;
        CheckHealth();
    }
}