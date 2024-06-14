using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(EnemyState))]
public class EnemyHealthManager : NetworkBehaviour, IDamageable
{
    [SerializeField] private int[] _lives;
    [SerializeField] private float _imuneTime;
    [SerializeField] private float _defenceTime;

    private EnemyState _enemyState;
    private EnemyHealthIndicator _healthIndicator;
    private SpriteRenderer _spriteRenderer;
    private float _defenceTimer;
    private float _imunityTimer;
    private int _maxHealth;

    public int Index { get; private set; }
    public int[] Lives { get => _lives; }


    private void Start()
    {
        Index = 0;
        _defenceTimer = 0;
        _imunityTimer = 0;
        _maxHealth = _lives[0];
        _enemyState = GetComponent<EnemyState>();
        _healthIndicator = GetComponent<EnemyHealthIndicator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!IsServer) return;
        if (_imunityTimer > 0)
            _imunityTimer -= Time.deltaTime;

        if (_defenceTimer > 0)
        {
            _defenceTimer -= Time.deltaTime;
            if ((int)_enemyState.DefenceType < (int)DefenceType.ShouldDefend)
                _enemyState.DefenceType = DefenceType.ShouldDefend;
        }
        else if (_enemyState.DefenceType == DefenceType.ShouldDefend)
        {
            _enemyState.DefenceType = DefenceType.NotDefending;
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
            SetHealth(- lastAmount + newAmount);
            lastAmount = newAmount;

            yield return null;
        }
    }

    public IEnumerator VisualEffect(Color color, float duration)
    {
        _spriteRenderer.color = color;
        yield return new WaitForSeconds(duration);
        _spriteRenderer.color = Color.white;
    }

    private IEnumerator StunEffect(float duration)
    {
        while (duration > 0)
        {
            _spriteRenderer.color = new Color(1f, 1f, 1f, 0.2f);
            yield return new WaitForSeconds(0.3f);
            _spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.3f);
            duration -= 0.6f;
        }
    }

    public IEnumerator Stun(float duration)
    {
        StartStunEffectServ(duration);
        _enemyState.Stop = true;
        yield return new WaitForSeconds(duration);
        _enemyState.Stop = false;
    }


    private void CheckHealth()
    {
        if (_lives[Index] > _maxHealth)
        {
            _lives[Index] = _maxHealth;
        }
        else if (_lives[Index] <= 0)
        {
            Index++;
            if (Index == _lives.Length)
            {
                DestroyObject();
            }
            else
            {
                _maxHealth = _lives[Index];
                _imunityTimer = _imuneTime;
                StopAllCoroutines();
                StartCoroutine(Stun(_imuneTime));

                EnemyWeaponHandler enemyWeaponHandler;
                TryGetComponent<EnemyWeaponHandler>(out enemyWeaponHandler);
                if (enemyWeaponHandler != null)
                {
                    enemyWeaponHandler.AssignWeapon();
                }
            }
        }
    }

    public bool DestroyObject()
    {
        EnemyWeaponHandler enemyWeaponHandler;
        TryGetComponent<EnemyWeaponHandler>(out enemyWeaponHandler);
        if (enemyWeaponHandler != null)
        {
            enemyWeaponHandler.DropWeapon();
        }
        Destroy(gameObject);
        return true;
    }


    public void Heal(int amount, float time)
    {
        if (_imunityTimer > 0 || Index >= _lives.Length) return;

        if (time == 0)
        {
            SetHealth(amount);
        }
        else
        {
            StartCoroutine(HealthCoroutine(amount, time));
        }
    }

    public void Damage(int amount, float time, bool crit, float stunDuration)
    {
        if (_imunityTimer > 0 || Index >= _lives.Length) return;

        bool stun = stunDuration > 0;
        _defenceTimer = _defenceTime;

        if (time == 0)
        {
            SetHealth(-amount);
        }
        else
        {
            StartCoroutine(HealthCoroutine(-amount, time));
        }

        if (stun)
            StartCoroutine(Stun(stunDuration));
        else
            StartCoroutine(VisualEffect(Color.black, 0.1f));

        Color color;
        Vector3 scale;

        if (crit)
        {
            color = Color.red;
            scale = new Vector3(1.3f, 1.3f, 0);
        }
        else if (Lives[Index] - amount <= 0 || stun)
        {
            color = new Color(255, 120, 0);
            scale = new Vector3(1.2f, 1.2f, 0);
        }
        else
        {
            color = Color.white;
            scale = new Vector3(1, 1, 0);
        }

        _healthIndicator.DisplayDamageIndicator(scale, color, amount);
    }


    [ServerRpc(RequireOwnership = false)]
    public void SetHealth(int amount)
    {
        //change health parce que flemme de faire une autre methode
        SetHealthObs(amount);
    }

    [ObserversRpc]
    private void SetHealthObs(int amount)
    {
        if (_imunityTimer > 0 || Index >= _lives.Length) return;

        _lives[Index] += amount;
        CheckHealth();
    }


    [ServerRpc(RequireOwnership = false)]
    private void StartStunEffectServ(float duration)
    {
        StartStunEffectObs(duration);
    }

    [ObserversRpc]
    private void StartStunEffectObs(float duration)
    {
        StartCoroutine(StunEffect(duration));
    }
}