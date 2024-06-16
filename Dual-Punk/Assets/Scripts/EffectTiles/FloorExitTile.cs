using System;
using System.Numerics;
using UnityEngine;

public class FloorExitTile : EffectTile
{
    public FloorExitTile(Vector3Int position) : base(position) {}

    public override void Action(GameObject target)
    {
        GameManager.Instance.FadeIn();
        if ((int)FloorNetworkWrapper.Instance.LocalFloorManager.CurrentFloor.FloorType+1 < FloorNetworkWrapper.Instance.LocalFloorManager.FloorTypeCount)
        {
            FloorNetworkWrapper.Instance.NewFloor(FloorNetworkWrapper.Instance.LocalFloorManager.CurrentFloor.FloorType+1);
        }
        else
        {
            FloorNetworkWrapper.Instance.StartEndFight();
        }
    }
}