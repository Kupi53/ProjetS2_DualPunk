
using UnityEngine;

public class TeleporterTile : EffectTile
{
    protected Vector3Int _targetPosition;
    
    public TeleporterTile(Vector3Int position, Vector3Int targetPosition) : base(position)
    {
        _targetPosition = targetPosition;
    }

    public override void Action(GameObject target)
    {
        Teleport(target, _targetPosition);
    }

    protected void Teleport(GameObject target, Vector3Int targetPosition)
    {
        target.transform.position = _targetPosition;
    }
}