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
    [SerializeField] protected float _dashReloadMultiplier;
    [SerializeField] protected float _dashSpeedMultiplier;

    private bool _isModified = false;

    private MouvementsController MouvementsController { get => PlayerState.gameObject.GetComponent<MouvementsController>(); }


    void Awake()
    {
        Type = ImplantType.Boots;
        SetName = "Heavy";
    }

    public override void Run()
    {
        if (IsEquipped && !_isModified)
        {
            _isModified = true;
            MouvementsController.SetDash(_dashSpeedMultiplier, _dashReloadMultiplier);
        }
    }

    public override void ResetImplant()
    {
        _isModified = false;
        MouvementsController.SetDash(1/_dashSpeedMultiplier, 1/_dashReloadMultiplier);

        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}
