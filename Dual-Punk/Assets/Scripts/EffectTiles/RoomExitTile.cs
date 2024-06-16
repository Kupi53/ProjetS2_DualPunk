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
        _targetCoordinates = ComputeTargetCoordinates(targetWall, _targetRoom);
    }

    public override void Action(GameObject target)
    {   
        GameManager.Instance.FadeIn();
        target.transform.position = _targetGrid.CellToWorld(_targetCoordinates);
        FloorNetworkWrapper.Instance.LocalFloorManager.CurrentRoom.Exit(_targetRoom);
    }

    public static Vector3Int ComputeTargetCoordinates(WallCardinal targetWallCardinal, Room targetRoom)
    {
        Vector3Int targetCoordinates;
        if (targetWallCardinal == targetRoom._entryWallCardinal)
        {
            targetCoordinates = targetRoom._entryWallCoordinates[targetRoom._entryWallCoordinates.Length/2];
        }
        else
        {
            targetCoordinates = targetRoom._exitWallCoordinates[targetRoom._exitWallCoordinates.Length/2];
        }
        switch(targetWallCardinal){
            case WallCardinal.North:
                targetCoordinates.x -= 2;
                break;
            case WallCardinal.East:
                targetCoordinates.y += 2;
                break;
            case WallCardinal.South:
                targetCoordinates.x += 2;
                break;
            case WallCardinal.West:
                targetCoordinates.y -= 2;
                break;
        }
        return targetCoordinates;
    }
}