using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using UnityEngine.Playables;
using Unity.VisualScripting;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;

public class LifeForce : ImplantScript
{
    [SerializeField] private int _extraLife;

    private bool _isModified = false;
    private int _oldMaxHealth;

    void Awake()
    {
        Type = ImplantType.ExoSqueleton;
        SetNumber = 2;
    }

    public override void Run()
    {
        if (IsEquipped && !_isModified)
        {
            _oldMaxHealth = PlayerState.MaxHealth;
            
            PlayerState.MaxHealth += _extraLife;
            PlayerState.Health *= PlayerState.MaxHealth;
            PlayerState.Health /= _oldMaxHealth;

            _isModified = true;
        }
    }

    public override void ResetImplant()
    {
        PlayerState.Health *= _oldMaxHealth;
        PlayerState.Health /= PlayerState.MaxHealth;
        PlayerState.MaxHealth -= _extraLife;
            
        _isModified = false;

        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}
