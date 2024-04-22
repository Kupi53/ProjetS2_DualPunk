
using UnityEngine;

public class EffectTile
{
    public Vector3Int Position {get;}

    public EffectTile(Vector3Int position)
    {
        Position = position;
    }

    public virtual void Action(GameObject target) {}
}