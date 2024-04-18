using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum WallCardinal{
    North,
    East,
    South,
    West,
}
public enum RoomType{
    FloorExit,
    FloorEntry,
    Normal
}

public class Room : MonoBehaviour
{
    public Room NextRoom;
    public Room PreviousRoom;
    private Vector3Int[] _entryWallCoordinates;
    private Vector3Int[] _exitWallCoordinates;
    private WallCardinal _exitWallCardinal;
    private WallCardinal _entryWallCardinal;
    private Floor _floor;
    [SerializeField] private int _roomPrefabId;
    private bool _isCleared;
    private RoomType _roomType
    { 
        get 
        {
            if (_floor.Entry == this)
            {
                return RoomType.FloorEntry;
            }
            else if (_floor.Exit == this)
            {
                return RoomType.FloorExit;
            }
            else 
            {
                return RoomType.Normal;
            }
        }
    }

    // called upon generation of eadch room (FloorManager.GenerateFloor); initialises variables, generates doors, spawns entities
    public void Init(Floor floor)
    {
        _floor = floor;
        _isCleared = false;
        GenerateWalls();
    }

    // deactives current room , activates the next by calling SwitchRoom
    public void Exit(){

    }

    private void GenerateWalls()
    {
        (_entryWallCardinal, _exitWallCardinal) = PickWalls();
        _entryWallCoordinates = FindWallCoordinates(_entryWallCardinal);
        _exitWallCoordinates = FindWallCoordinates(_exitWallCardinal);
        Tilemap wallTilemap = this.gameObject.transform.GetChild(FloorManager.TILEMAPLAYERINDEX).gameObject.GetComponent<Tilemap>();
        foreach ( Vector3Int coordinate in _entryWallCoordinates)
        {
            wallTilemap.SetTile(coordinate, FloorManager.Instance.test1);
        }
        foreach ( Vector3Int coordinate in _exitWallCoordinates)
        {
            wallTilemap.SetTile(coordinate, FloorManager.Instance.test2);
        }
    }

    private static WallCardinal GetOppositeWall(WallCardinal cardinal)
    {
        switch (cardinal)
        {
            case WallCardinal.North:
                return WallCardinal.South;
            case WallCardinal.East:
                return WallCardinal.West;
            case WallCardinal.South:
                return WallCardinal.North;
            case WallCardinal.West:
                return WallCardinal.East;
            default:
                throw new Exception("cardinal is not valid or implemented in this function");
        }
    }

    private (WallCardinal, WallCardinal) PickWalls()
    {
        WallCardinal entryWall;
        if (_roomType == RoomType.FloorEntry)
        {
            entryWall = (WallCardinal)UnityEngine.Random.Range(0, 4);
        }
        else 
        {
            Debug.Log(_roomPrefabId);
            Debug.Log(_roomType.ToString());
            entryWall = GetOppositeWall(PreviousRoom._exitWallCardinal);
        }
        WallCardinal exitWall = (WallCardinal)UnityEngine.Random.Range(0, 4);
        while (exitWall == entryWall)
        {
            exitWall = (WallCardinal)UnityEngine.Random.Range(0, 4);
        }
        return (entryWall, exitWall);
    }
    
    // finds the coordinates of each tile of the corresponding wall 
    private Vector3Int[] FindWallCoordinates(WallCardinal wallCardinal)
    {
        List<Vector3Int> wallCoordinates = new List<Vector3Int>();

        Tilemap wallTilemap = this.gameObject.transform.GetChild(FloorManager.DATALAYERINDEX).GetChild((int)wallCardinal).gameObject.GetComponent<Tilemap>();
        foreach (var point in wallTilemap.cellBounds.allPositionsWithin)
        {
            if (wallTilemap.HasTile(point))
            {
                wallCoordinates.Add(new Vector3Int(point.x, point.y, point.z));
            }
        }

        return ListToArrayVector3Int(wallCoordinates);
    }

    private static Vector3Int[] ListToArrayVector3Int (List<Vector3Int> list)
    {
        Vector3Int[] array = new Vector3Int[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            array[i] = list[i];
        }
        return array;
    }
}
