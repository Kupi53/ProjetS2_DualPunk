using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using UnityEngine.Playables;
using Unity.VisualScripting;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;

public class SwiftDash : ImplantScript
{
    [SerializeField] protected float _speedReloadDash;

    private bool _isModified = false;

    private MouvementsController MouvementsController
    {
        get
        {
            return PlayerState.gameObject.GetComponent<MouvementsController>();
        }
    }

    void Awake()
    {
        Type = ImplantType.Boots;
        SetNumber = 2;
    }

    public override void Run()
    {
        if (IsEquipped && !_isModified)
        {
            MouvementsController.DashCooldown /= _speedReloadDash;

            _isModified = true;
        }
    }

    public override void ResetImplant()
    {
        MouvementsController.DashCooldown *= _speedReloadDash;

        _isModified = false;

        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}
