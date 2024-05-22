using System.Numerics;
using UnityEngine;

public class FloorExitTile : EffectTile
{
    public FloorExitTile(Vector3Int position) : base(position) {}

    public override void Action(GameObject target)
    {
        if (!target.GetComponent<PlayerState>().CanBeTeleported) return;
        GameManager.Instance.FadeIn();
        FloorNetworkWrapper.Instance.NewFloor(FloorNetworkWrapper.Instance.LocalFloorManager.CurrentFloor.FloorType+1);
    }
}