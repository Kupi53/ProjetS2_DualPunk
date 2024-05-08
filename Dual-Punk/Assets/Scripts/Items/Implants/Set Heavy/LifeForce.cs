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
    private int _oldHealth;

        void Awake()
    {
        Type = ImplantType.ExoSqueleton;
        SetNumber = 2;
    }

    public override void Run()
    {
        if (IsEquipped && !_isModified)
        {
            _oldHealth = PlayerState.Health;

            PlayerState.MaxHealth += _extraLife;
            PlayerState.Health += _extraLife;

            _isModified = true;
        }
    }

    public override void ResetImplant()
    {
        PlayerState.MaxHealth -= _extraLife;

        if (PlayerState.Health > _oldHealth)
        {
            PlayerState.Health -= _extraLife;
        }
            
        _isModified = false;

        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}
