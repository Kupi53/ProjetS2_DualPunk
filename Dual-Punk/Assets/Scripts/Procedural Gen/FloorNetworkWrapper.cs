using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Utility.Extension;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorNetworkWrapper : NetworkBehaviour
{
    private const int _baseMinEnemies = 6;
    private const int _baseMaxEnemies = 11;
    private int _actualMinEnemies
    {
        get
        {
            return (int)(_baseMinEnemies * (int)(LocalFloorManager.CurrentFloor.FloorType+1) * 0.5f);
        }
    }
    private int _actualMaxEnemies
    {
        get
        {
            return (int)(_baseMaxEnemies * (int)(LocalFloorManager.CurrentFloor.FloorType+1) * 0.5f);
        }
    }
    public static FloorNetworkWrapper Instance;
    [SerializeField] private GameObject _floorManagerPrefab;
    public FloorManager LocalFloorManager {
        get
        {
            return GameObject.FindWithTag("FloorManager").GetComponent<FloorManager>();
        }
    }

    override public void OnStartNetwork(){
        base.OnStartNetwork();
        if (Instance==null)
        {
            Instance = this;
        }
        Instantiate(_floorManagerPrefab);
    }

    void Update()
    {
        if (LocalFloorManager != null && LocalFloorManager.CurrentRoom != null)
        {
            CheckEnemiesOOB();
        }
    }

    public void NewFloor(FloorType floorType)
    {
        if (!IsServer) return;
        
        int seed = UnityEngine.Random.Range(0,9999);
        SeedRPC(seed);

        if (LocalFloorManager.CurrentFloor != null)
        {
            DestroyCurrentFloorRPC();
        }
        SwitchToNewFloorRPC(floorType);
    }

    public void SpawnEnemies()
    {
        if (!IsServer) return;
        Debug.Log(LocalFloorManager);
        Tilemap[] tilemaps = LocalFloorManager.CurrentRoom.GetComponentsInChildren<Tilemap>();
        int enemyAmount = UnityEngine.Random.Range(_actualMinEnemies, _actualMaxEnemies+1);
        Tilemap tileMap = tilemaps.Where(map => map.gameObject.name == "Tilemap").First();
        IEnumerable<Tilemap> elevationMaps = tilemaps.Where(map => map.gameObject.name.StartsWith("Elevation"));
        BoundsInt bounds = tileMap.cellBounds;
        // pick random enemies from the prefab list and assign random positions to them (that or not out of bounds (cellbound) and do not collide with anything (cant be on an elevation))
        for (int i = 0; i < enemyAmount; i++)
        {
            int enemyPrefab = UnityEngine.Random.Range(0, LocalFloorManager.CurrentFloor.EnnemyPrefabs.Length);
            Vector3 WorldPosition = FindSuitablePosition(tileMap, elevationMaps, bounds);
            GameObject Enemy = Instantiate(LocalFloorManager.CurrentFloor.EnnemyPrefabs[enemyPrefab], WorldPosition, quaternion.identity);
            Spawn(Enemy);
        }
        PopulateRoomEnemiesRpc();
    }
    public void SpawnLoot()
    {
        if (!IsServer) return;
        SpawnLootRpc();
    }

    [ObserversRpc]
    private void SeedRPC(int seed)
    {
        UnityEngine.Random.InitState(seed);
    }
    [ObserversRpc]
    private void DestroyCurrentFloorRPC()
    {
        if (LocalFloorManager.CurrentFloor != null)
        {
            LocalFloorManager.CurrentFloor.DestroyHolder();
        }
    }
    [ObserversRpc]
    private void SwitchToNewFloorRPC(FloorType floorType)
    {
        LocalFloorManager.CurrentFloor = LocalFloorManager.GenerateFloor(floorType);
        LocalFloorManager.SwitchRoom(LocalFloorManager.CurrentFloor.Entry);
        GameManager.Instance.LocalPlayer.transform.position = 
        LocalFloorManager.CurrentFloor.Entry.GetComponent<Grid>().CellToWorld(RoomExitTile.ComputeTargetCoordinates(
        Instance.LocalFloorManager.CurrentFloor.Entry._entryWallCardinal, 
        Instance.LocalFloorManager.CurrentFloor.Entry));
    }

    [ServerRpc (RequireOwnership = false)]
    public void SpawnLootRpc()
    {
        
    }

    [ObserversRpc]
    private void PopulateRoomEnemiesRpc()
    {
        LocalFloorManager.CurrentRoom.PopulateRoomEnemies();
    }

    public void CheckEnemiesOOB()
    {
        if (!IsServer) return;
        Tilemap[] tilemaps = LocalFloorManager.CurrentRoom.GetComponentsInChildren<Tilemap>();
        Tilemap tileMap = tilemaps.Where(map => map.gameObject.name == "Tilemap").First();
        IEnumerable<Tilemap> elevationMaps = tilemaps.Where(map => map.gameObject.name.StartsWith("Elevation"));
        BoundsInt bounds = tileMap.cellBounds;
        foreach (GameObject enemy in LocalFloorManager.CurrentRoom.Enemies)
        {
            Vector3Int enemyPos = tileMap.WorldToCell(enemy.transform.position);
            if (!tileMap.HasTile(enemyPos) && !elevationMaps.Any(t=>t.HasTile(enemyPos)))
            {
                TeleportInBounds(enemy, tileMap, elevationMaps, bounds);
            }
        }
    }

    private void TeleportInBounds(GameObject enemy, Tilemap tilemap, IEnumerable<Tilemap> elevationMaps, BoundsInt bounds)
    {
        Vector3 newPos = FindSuitablePosition(tilemap, elevationMaps, bounds);
        enemy.transform.position = newPos;
    }

    private Vector3 FindSuitablePosition(Tilemap tileMap, IEnumerable<Tilemap> elevationMaps, BoundsInt bounds)
    {
        int posX = UnityEngine.Random.Range(bounds.xMin, bounds.xMax);
        int posY = UnityEngine.Random.Range(bounds.yMin, bounds.yMax);
        Vector3Int position = new Vector3Int(posX, posY,0);
        while (tileMap.GetTile(position) == null || elevationMaps.Any(map => map.GetTile(position) != null))
        {
            posX = UnityEngine.Random.Range(bounds.xMin, bounds.xMax);
            posY = UnityEngine.Random.Range(bounds.yMin, bounds.yMax);
            position = new Vector3Int(posX, posY,0);
        }
        return tileMap.CellToWorld(position);
    }
}