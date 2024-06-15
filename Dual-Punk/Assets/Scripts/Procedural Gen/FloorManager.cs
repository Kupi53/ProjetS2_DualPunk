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
    Hangar
}


public class FloorManager : MonoBehaviour
{
    public Floor CurrentFloor;
    public Room CurrentRoom{
        get 
        {
           GameObject activeRoom = GameObject.FindWithTag("ActiveRoom");
           if (activeRoom == null) return null;
           else return activeRoom.GetComponent<Room>();
        }
    }
    [SerializeField] public GameObject[] CityRoomPrefabs;
    [SerializeField] public GameObject[] HangarRoomPrefabs;
    [SerializeField] public GameObject[] CityEnemyPrefabs;
    [SerializeField] public GameObject[] HangarEnemyPrefabs;
    [SerializeField] public GameObject FloorExitWallPrefab;
    public const int DATALAYERINDEX = 0;
    public const int TILEMAPLAYERINDEX = 1;
    public const int ELEVATIONLAYERINDEX = 2;
    public const int COLLISIONMAPLAYERINDEX = 3;
    [SerializeField] public Tile test1;
    [SerializeField] public Tile test2;
    //
    public int FloorTypeCount { get => Enum.GetNames(typeof(FloorType)).Length; }
    private const int _minRoomAmount = 4;
    private const int _maxRoomAmount = 8;


    // pour le debug
    void Update(){
        // A SUPRIMER
        if (Input.GetKeyDown(KeyCode.G)){
            FloorNetworkWrapper.Instance.NewFloor(FloorType.Hangar);
        }
    }

    // creates a floor and spawns all rooms, then returns that floor
    public Floor GenerateFloor(FloorType floorType)
    {
        // create the parent holder
        GameObject CurrentFloorHolder = new GameObject("CurrentFloorHolder");
        // create the new floor
        Floor floor = new Floor(floorType);
        floor.FloorHolderObject = CurrentFloorHolder;
        int actualMinRoomAmount = _minRoomAmount + ((int)floorType * 2);
        int actualMaxRoomAmount = _maxRoomAmount + ((int)floorType * 3);
        // pick the amount of rooms
        int roomAmount = UnityEngine.Random.Range(actualMinRoomAmount, actualMaxRoomAmount);
        // add rooms
        for (int i = 0; i < roomAmount; i++)
        {
            // pick the prefab id
            int roomPrefabId = UnityEngine.Random.Range(0, floor.RoomPrefabs.Length);
            // instantiate the room
            GameObject newRoomObject = Instantiate(floor.RoomPrefabs[roomPrefabId]);
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
        if (!newRoom.Visited)
        {
            FloorNetworkWrapper.Instance.SpawnEnemies();
            newRoom.Visited = true;
        }
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
