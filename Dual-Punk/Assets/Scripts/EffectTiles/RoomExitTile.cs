using System.Numerics;
using UnityEngine;

public class RoomExitTile : EffectTile
{
    private Room _targetRoom;
    private Grid _targetGrid {get => _targetRoom.gameObject.GetComponent<Grid>();}
    private Vector3Int _targetCoordinates;

    public RoomExitTile(Vector3Int position, Room targetRoom, WallCardinal targetWall) : base(position)
    {
        _targetRoom = targetRoom;
        _targetCoordinates = ComputeTargetCoordinates(targetWall);
    }

    public override void Action(GameObject target)
    {     
        if (!target.GetComponent<PlayerState>().CanBeTeleported) return;
        target.transform.position = _targetGrid.CellToWorld(_targetCoordinates);
        FloorManager.Instance._currentRoom.Exit(_targetRoom);
    }

    private Vector3Int ComputeTargetCoordinates(WallCardinal targetWallCardinal)
    {
        Vector3Int targetCoordinates;
        if (targetWallCardinal == _targetRoom._entryWallCardinal)
        {
            targetCoordinates = _targetRoom._entryWallCoordinates[_targetRoom._entryWallCoordinates.Length/2];
        }
        else
        {
            targetCoordinates = _targetRoom._exitWallCoordinates[_targetRoom._exitWallCoordinates.Length/2];
        }
        return targetCoordinates;
    }
}