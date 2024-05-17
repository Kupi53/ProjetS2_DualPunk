using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Managing.Client;
using FishNet.Managing.Server;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum FloorType{
    City,
    Hangar,
    Spaceship
}


public class FloorManager : NetworkBehaviour
{
    public static FloorManager Instance;
    public Floor CurrentFloor;
    public Room CurrentRoom{
        get {
            return GameObject.FindWithTag("ActiveRoom").GetComponent<Room>();
        }
    }
    [SerializeField] public GameObject[] CityRoomPrefabs;
    [SerializeField] public GameObject[] HangarRoomPrefabs;
    [SerializeField] public GameObject[] SpaceshipRoomPrefabs;
    public const int DATALAYERINDEX = 0;
    public const int TILEMAPLAYERINDEX = 1;
    public const int ELEVATIONLAYERINDEX = 2;
    public const int COLLISIONMAPLAYERINDEX = 3;
    [SerializeField] public Tile test1;
    [SerializeField] public Tile test2;
    //
    private int _floorTypeCount { get => Enum.GetNames(typeof(FloorType)).Length; }
    private int _minRoomAmount = 5;
    private int _maxRoomAmount = 9;


    // le serveur spawn et definit l'instance du floormanager
    void Awake()
    {
        Instance = this;
    }

    // pour le debug
    void Update(){
        if (Input.GetKeyDown(KeyCode.G)){
            NewFloor(FloorType.City);
        }
    }

    // creates a floor and spawns all rooms, then returns that floor
    private Floor GenerateFloor(FloorType floorType)
    {
        // create the parent holder
        GameObject CurrentFloorHolder = new GameObject("CurrentFloorHolder");
        // create the new floor
        Floor floor = new Floor(floorType);
        floor.FloorHolderObject = CurrentFloorHolder;
        // pick the amount of rooms
        int roomAmount = UnityEngine.Random.Range(_minRoomAmount, _maxRoomAmount+1);
        // add rooms
        for (int i = 0; i < roomAmount; i++)
        {
            // pick the prefab id
            int roomPrefabId = UnityEngine.Random.Range(0, floor.RoomPrefabs.Length);
            // instantiate the room
            GameObject newRoomObject = Instantiate(CityRoomPrefabs[roomPrefabId]);
            newRoomObject.transform.SetParent(CurrentFloorHolder.transform);
            // setup the gameobject's name
            if (i == 0)
            {
                newRoomObject.name = "EntryRoom";
            }
            else if (i == roomAmount-1)
            {
                newRoomObject.name = "ExitRoom";
            }
            else
            {
                newRoomObject.name = $"Room{i}";
            }
            // initialise the room component with the correct variables
            Room newRoom = newRoomObject.GetComponent<Room>();
            // append this room to the floor we're creating
            floor.AppendRoom(newRoom);
            // this depends on knowing wether it is the floor's entry, exit or normal room so it needs to be called after append
            newRoom.Init(floor);
        }
        // need to go back through the spawned rooms and spawn all the exit teleporter tiles and also deactive them (except the entry)
        Room room = floor.Entry;
        room.SpawnExits();
        room = room.NextRoom;
        while (room != null)
        {
            room.SpawnExits();
            room.gameObject.SetActive(false);
            room = room.NextRoom;
        }
        return floor;
    }



    // Activates the new room, places the player at the entry
    public void SwitchRoom(Room newRoom)
    {
        newRoom.gameObject.SetActive(true);
        newRoom.tag = "ActiveRoom";
    }

    // Creates a new floor and switches to it's entry room
    public void NewFloor(FloorType floorType)
    {
        if (!IsServer) return;
        
        int seed = UnityEngine.Random.Range(0,9999);
        SeedRPC(seed);

        if (CurrentFloor != null)
        {
            DestroyCurrentFloorRPC();
        }
        SwitchToNewFloorRPC(floorType);
    }

    [ObserversRpc]
    private void SeedRPC(int seed)
    {
        Debug.Log("fkldjajlf");
        UnityEngine.Random.InitState(seed);
    }
    [ObserversRpc]
    private void DestroyCurrentFloorRPC()
    {
        CurrentFloor.DestroyHolder();
    }
    [ObserversRpc]
    private void SwitchToNewFloorRPC(FloorType floorType)
    {
        CurrentFloor = GenerateFloor(floorType);
        SwitchRoom(CurrentFloor.Entry);
    }
    // For testing purposes, Goes through the _currentFloor and converts attributes to a string, then debug.logs it
/*
    private void PrintFloor()
    {
        string res = "Current Floor :\n";
        res+= $"    Entry :\n";
        Room currentRoom = _currentFloor.Entry;
        int i = 1;
        while (currentRoom is not null)
        {
            res+=$"        Id : {currentRoom._roomPrefabId}, ";
            if (currentRoom.PreviousRoom is not null)
            {
                res += $"Previous Room Id : {currentRoom.PreviousRoom._roomPrefabId}, ";
            }
            if (currentRoom.NextRoom is not null)
            {
                res += $"Next Room Id : {currentRoom.NextRoom._roomPrefabId}\n";
                if (currentRoom.NextRoom._roomType == RoomType.FloorExit)
                {
                    res += "    Exit :\n";
                }
                else 
                {
                    res += $"    Room {i} : \n";
                    i++;
                }
            }
            currentRoom = currentRoom.NextRoom;
        }
        Debug.Log(res);
    }
*/
}
