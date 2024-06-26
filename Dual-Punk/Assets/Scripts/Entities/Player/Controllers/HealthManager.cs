using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;


public class HealthManager : NetworkBehaviour, IDamageable
{
    private PlayerState _playerState;
    private ReviveAndDownController _reviveAndDownController;

    //Nécessaire pour implant ThermicExchange
    public float DamageMultiplier { get; set; }
    public float DamageToSecond { get; set; }
    #nullable enable
    public LaserGunScript? LaserGunScript { get; set; }
    #nullable disable

    //Nécessaire pour implant Bulletstorm
    public bool DodgeActive { get; set; } = false;
    public int DodgePercentage {get; set; }
    [SerializeField] private GameObject _floatingTextPrefab;
    [SerializeField] private GameObject _floatingTextsParent;

    //Nécessaire pour implant TeleportStrike
    public bool Teleportation { get; set; } = false;


    private void Start()
    {
        _reviveAndDownController = GetComponent<ReviveAndDownController>();
        _playerState = GetComponent<PlayerState>();
    }


    private void CheckHealth()
    {
        if (_playerState.Health > _playerState.MaxHealth)
            _playerState.Health = _playerState.MaxHealth;

        else if (_playerState.Health <= 0)
        {
            _reviveAndDownController.DownServer();
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
    }


    // tous les degats (ou heal) doivent etre inflige par une fonction sur le serv
    [ObserversRpc]
    public void Heal(int amount, float time)
    {
        if (_playerState.IsDown) return;

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
    public void Damage(int amount, float time, bool crit, float stunDuration)
    {
        if (!Teleportation && !_playerState.IsDown)
        {
            if (_playerState.Dashing || (DodgeActive && UnityEngine.Random.Range(0, 100) < DodgePercentage))
            {
                DisplayMessageIndicator("Dodge", new Vector3(1, 1, 0), Color.white);
            }
            else
            {
                float newAmout = amount;

                ImplantController implantController = gameObject.GetComponent<ImplantController>();
                (bool IsActive, string SetName) = implantController.SetIsEquipped();

                if (IsActive && SetName == "Heavy")
                {
                    newAmout -= implantController.LessDamageMultiplier * newAmout;
                }
                    

                if (LaserGunScript != null && LaserGunScript.CoolDownLevel < LaserGunScript.FireTime / 2)
                {
                    newAmout *= DamageMultiplier;
                    float addedTime = (amount / DamageToSecond) * LaserGunScript.FireTime;
                    if (LaserGunScript.CoolDownLevel + addedTime > LaserGunScript.FireTime)
                    {
                        LaserGunScript.CoolDownLevel = LaserGunScript.FireTime;
                    }
                    else
                    {
                        LaserGunScript.CoolDownLevel += addedTime;
                    }
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
        }
    }


    public void SetHealth(int amount)
    {
        _playerState.Health = amount;
        CheckHealth();
    }

    public bool DestroyObject()
    {
        _playerState.Health = 0;
        CheckHealth();
        return true;
    }


    public void DisplayMessageIndicator(string message, Vector3 scale, Color color)
    {
        if (message == "") return;

        GameObject floatingText = Instantiate(_floatingTextPrefab);
        floatingText.transform.SetParent(_floatingTextsParent.transform);
        floatingText.GetComponent<RectTransform>().localScale = scale;
        floatingText.GetComponent<TextMeshProUGUI>().text = message;
        floatingText.GetComponent<TextMeshProUGUI>().color = color;
    }
}