using UnityEngine;
using FishNet.Object;

public class FuryEnhancer : ImplantScript
{
    [SerializeField] protected float _speedAttackRate;
    [SerializeField] protected float _multiplierDamage;
    
    void Awake()
    {
        Type = ImplantType.Arm;
        SetName = "Scavenger";
    }

    public override void Run()
    {
        
    }

    public override void ResetImplant()
    {
        
    }
}
