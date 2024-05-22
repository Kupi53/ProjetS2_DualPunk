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
    [SerializeField] private float _multiplier;

    private bool _isModified;
    private int _addedHealth;


    void Awake()
    {
        _isModified = false;
        Type = ImplantType.ExoSqueleton;
        SetName = "Heavy";
    }

    public override void Run()
    {
        if (IsEquipped && !_isModified)
        {
            _addedHealth = (int)(PlayerState.MaxHealth * _multiplier - PlayerState.MaxHealth);
            PlayerState.MaxHealth += _addedHealth;
            _isModified = true;
        }
    }

    public override void ResetImplant()
    {
        PlayerState.MaxHealth -= _addedHealth;          
        _isModified = false;

        if (PlayerState.Health > PlayerState.MaxHealth)
            PlayerState.Health = PlayerState.MaxHealth;

        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}
