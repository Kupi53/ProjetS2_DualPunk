using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Vector3 = UnityEngine.Vector3;

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
    public Vector3Int[] _entryWallCoordinates;
    public Vector3Int[] _exitWallCoordinates;
    public WallCardinal _exitWallCardinal;
    public WallCardinal _entryWallCardinal;
    public List<GameObject> Enemies;
    public bool IsCleared 
    {
        get => Enemies.Count == 0;
    }
    public bool Visited;
    [SerializeField] private int _roomPrefabId;

    private EffectTilesController _effectTilesController;
    private Floor _floor;
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
        _effectTilesController = this.gameObject.GetComponent<EffectTilesController>();
        _effectTilesController.EffectTiles = new List<EffectTile>();
        GenerateWalls();
    }
    public void SpawnExits()
    {
        Tilemap wallTilemap = this.gameObject.transform.GetChild(FloorManager.TILEMAPLAYERINDEX).gameObject.GetComponent<Tilemap>();
        if (_roomType != RoomType.FloorEntry)
        {
            foreach ( Vector3Int coordinate in _entryWallCoordinates)
            {
                wallTilemap.SetTile(coordinate, FloorNetworkWrapper.Instance.LocalFloorManager.test1);
                _effectTilesController.EffectTiles.Add(new RoomExitTile(coordinate, PreviousRoom, GetOppositeWall(_entryWallCardinal)));
            }
        }
        if (_roomType != RoomType.FloorExit)
        {
            foreach ( Vector3Int coordinate in _exitWallCoordinates)
            {
                wallTilemap.SetTile(coordinate, FloorNetworkWrapper.Instance.LocalFloorManager.test1);
                _effectTilesController.EffectTiles.Add(new RoomExitTile(coordinate, NextRoom, GetOppositeWall(_exitWallCardinal)));
            }
        }
        else
        {
            GameObject floorExitWall = Instantiate(FloorNetworkWrapper.Instance.LocalFloorManager.FloorExitWallPrefab);
            floorExitWall.transform.SetParent(gameObject.transform);
            wallTilemap = floorExitWall.GetComponent<Tilemap>();
            foreach ( Vector3Int coordinate in _exitWallCoordinates)
            {
                wallTilemap.SetTile(coordinate, FloorNetworkWrapper.Instance.LocalFloorManager.test2);
                _effectTilesController.EffectTiles.Add(new FloorExitTile(coordinate));
            }
        }

    }

    // deactives current room , activates the next by calling SwitchRoom
    public void Exit(Room targetRoom)
    {
        this.gameObject.SetActive(false);
        this.gameObject.tag = "ActiveRoom";
        FloorNetworkWrapper.Instance.LocalFloorManager.SwitchRoom(targetRoom);
    }

    private void GenerateWalls()
    {
        (_entryWallCardinal, _exitWallCardinal) = PickWalls();
        _entryWallCoordinates = FindWallCoordinates(_entryWallCardinal);
        _exitWallCoordinates = FindWallCoordinates(_exitWallCardinal);
    }
    public void PopulateRoomEnemies()
    {
        Enemies = GameObject.FindGameObjectsWithTag("Ennemy").ToList();
        foreach(GameObject enemy in Enemies)
        {
            enemy.GetComponent<EnemyState>().ParentRoom = this;
            enemy.transform.SetParent(this.gameObject.transform);
        }
    }
    public void OnEnemyDeath()
    {
        if (IsCleared)
        {   
            GameManager.Instance.RoomsCleared += 1;
            Vector3 LootboxPos = SpawnLootBox();
            if (!StoryManager.Instance.StoryCompleted)
            {
                Tilemap[] tilemaps = FloorNetworkWrapper.Instance.LocalFloorManager.CurrentRoom.GetComponentsInChildren<Tilemap>();
                Tilemap tileMap = tilemaps.Where(map => map.gameObject.name == "Tilemap").First();
                IEnumerable<Tilemap> elevationMaps = tilemaps.Where(map => map.gameObject.name.StartsWith("Elevation"));
                BoundsInt bounds = tileMap.cellBounds;
                Vector3 pos = FloorNetworkWrapper.Instance.FindSuitablePosition(tileMap, elevationMaps, bounds);
                StoryManager.Instance.SpawnNpc(pos);
            }
        }   
    }

    private Vector3 SpawnLootBox()
    {
        Tilemap[] tilemaps = FloorNetworkWrapper.Instance.LocalFloorManager.CurrentRoom.GetComponentsInChildren<Tilemap>();
        Tilemap tileMap = tilemaps.Where(map => map.gameObject.name == "Tilemap").First();
        IEnumerable<Tilemap> elevationMaps = tilemaps.Where(map => map.gameObject.name.StartsWith("Elevation"));
        BoundsInt bounds = tileMap.cellBounds;
        int posX = UnityEngine.Random.Range(bounds.xMin, bounds.xMax);
        int posY = UnityEngine.Random.Range(bounds.yMin, bounds.yMax);
        Vector3Int position = new Vector3Int(posX, posY,0);
        bool found = false;
        while (!found)
        {
            bool candididate = true;
            posX = UnityEngine.Random.Range(bounds.xMin, bounds.xMax);
            posY = UnityEngine.Random.Range(bounds.yMin, bounds.yMax);
            position = new Vector3Int(posX, posY,0);
            for (int i = -3; i<=3 && candididate; i++)
            {
                for (int j = -3; j<=3 && candididate; j++)
                {
                    if (tileMap.GetTile(position + new Vector3Int(i, j, 0)) == null || elevationMaps.Any(map => map.GetTile(position + new Vector3Int(i,j,0)) != null))
                    {
                        candididate = false;
                    }
                }
            }
            found = candididate;
        }
        Vector3 WorldPosition = tileMap.CellToWorld(position);
        ObjectSpawner.Instance.SpawnObjectFromIdRpc("0030", WorldPosition, quaternion.identity);
        return WorldPosition;
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
