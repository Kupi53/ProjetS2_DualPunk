using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;


public class LaserTracker : ImplantScript
{
    [SerializeField] private float _lockingRange;
    [SerializeField] private float _aimingTime;

    private AttacksController AttacksController => PlayerState.GetComponent<AttacksController>();
    
    
    void Awake()
    {
        Type = ImplantType.Neuralink;
        SetName = "Laser";
    }

    public override void Run()
    {
        if (IsEquipped && PlayerState.HoldingWeapon && PlayerState.WeaponScript is LaserGunScript)
        {
            AttacksController.SetLaserTracker(true, _lockingRange, _aimingTime);
        }
        else
        {
            AttacksController.SetLaserTracker(false, 0, 0);
        }
    }
    

    public override void ResetImplant()
    {
        AttacksController.SetLaserTracker(false, 0, 0);
        RemoveAllOwnerShipRPC(GetComponent<NetworkObject>());
    }
}
