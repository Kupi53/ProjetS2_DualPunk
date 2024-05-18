using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyHealthManager : MonoBehaviour, IDamageable
{
    [SerializeField] private int[] _lives;
    [SerializeField] private float _effectSmoothTime;
    [SerializeField] private float _receivedDamageFrequency;
    [SerializeField] private GameObject _sliderHealthObject;
    [SerializeField] private GameObject _sliderEffectObject;

    private Slider _sliderHealth;
    private Slider _sliderEffect;
    private int _lifeIndex;
    private int _maxHealth;
    private float _vel;


    private void Start()
    {
        _vel = 0;
        _lifeIndex = 0;
        _maxHealth = _lives[0];
        _sliderHealth = _sliderHealthObject.GetComponent<Slider>();
        _sliderEffect = _sliderEffectObject.GetComponent<Slider>();
    }

    private void Update()
    {
        _sliderHealth.value = (float)_lives[_lifeIndex] / (float)_maxHealth;
        _sliderEffect.value = Mathf.SmoothDamp(_sliderEffect.value, _sliderHealth.value, ref _vel, _effectSmoothTime);
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
            SetHealth(_lives[_lifeIndex] - lastAmount + newAmount);
            lastAmount = newAmount;

            yield return null;
        }

        SetHealth(_lives[_lifeIndex] - lastAmount + amount);
    }

    private IEnumerator VisualIndicator(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.15f);
        GetComponent<SpriteRenderer>().color = Color.white;
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
                DestroyObject();
            }
            else
            {
                _maxHealth = _lives[_lifeIndex];
                //event pour assign weapon (weaponhandler)
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
            StartCoroutine(VisualIndicator(Color.black));
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