using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyHealthManager : MonoBehaviour, IDamageable
{
    [SerializeField] private int[] _lives;
    [SerializeField] private float _receivedDamageFrequency;

    private int _lifeIndex;
    private int _maxHealth;

    public int[] Lives { get => _lives; }


    private void Start()
    {
        _lifeIndex = 0;
        _maxHealth = _lives[0];
    }


    private IEnumerator HealthCoroutine(int amount, float time)
    {
        int startHealth = _lives[_lifeIndex];
        int damageNumber = (int)(time / _receivedDamageFrequency);
        int counter = 0;
        float timer = 0;

        while (counter < damageNumber)
        {
            if (timer < _receivedDamageFrequency)
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
        GetComponent<SpriteRenderer>().color = Color.green;
    }


    private void CheckHealth()
    {
        if (_lives[_lifeIndex] > _maxHealth)
            _lives[_lifeIndex] = _maxHealth;

        else if (_lives[_lifeIndex] <= 0)
        {
            _lifeIndex++;
            if (_lifeIndex == _lives.Length)
            {
                //event pour drop (weaponhandler)
                Destroy();
            }
            else
            {
                _maxHealth = _lives[_lifeIndex];
                //event pour assign weapon (weaponhandler)
                GetComponent<EnemyWeaponHandler>().AssignWeapon();
            }
        }
    }


    public bool Destroy()
    {
        Destroy(gameObject);
        return true;
    }


    public void Heal(int amount, float time)
    {
        if (time == 0)
        {
            _lives[_lifeIndex] += amount;
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
            _lives[_lifeIndex] -= amount;
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
        if (amount < _lives[_lifeIndex])
        {
            StartCoroutine(VisualIndicator(Color.black));
        }

        _lives[_lifeIndex] = amount;
        CheckHealth();
    }
}