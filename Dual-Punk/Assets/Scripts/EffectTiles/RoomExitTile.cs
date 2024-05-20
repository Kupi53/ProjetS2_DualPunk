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
        Debug.Log("test");
        if (!target.GetComponent<PlayerState>().CanBeTeleported) return;
        Debug.Log("test");
        GameManager.Instance.FadeIn();
        target.transform.position = _targetGrid.CellToWorld(_targetCoordinates);
        FloorNetworkWrapper.Instance.LocalFloorManager.CurrentRoom.Exit(_targetRoom);
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