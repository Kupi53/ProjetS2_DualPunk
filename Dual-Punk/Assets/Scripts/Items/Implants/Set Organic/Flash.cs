using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using UnityEngine.Playables;
using Unity.VisualScripting;
using FishNet.Object;
using FishNet.Demo.AdditiveScenes;

public class Flash : ImplantScript
{
    [SerializeField] protected float _speedMultiplier;

    private bool _isModified = false;

    MouvementsController MouvementsController
    {
        get
        {
            return PlayerState.gameObject.GetComponent<MouvementsController>();
        }
    }

    void Awake()
    {
        Type = ImplantType.Boots;
    }
    
    public override void Run()
    {
        if (IsEquipped && !_isModified)
        {
            MouvementsController.WalkSpeed *= _speedMultiplier;
            MouvementsController.SprintSpeed *= _speedMultiplier;

            _isModified = true;
        }
    }

    public override void ResetImplant()
    {
        MouvementsController.WalkSpeed /= _speedMultiplier;
        MouvementsController.SprintSpeed /= _speedMultiplier;

        _isModified = false;

        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}
