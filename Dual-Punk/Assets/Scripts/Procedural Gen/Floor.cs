using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Floor
{
    public Room Entry;
    public Room Exit;
    public GameObject[] RoomPrefabs 
    { 
        get 
        {
            switch (_floorType){
                case FloorType.City:
                    return FloorManager.Instance.CityRoomPrefabs;
                case FloorType.Hangar:
                    return FloorManager.Instance.HangarRoomPrefabs;
                case FloorType.Spaceship:
                    return FloorManager.Instance.SpaceshipRoomPrefabs;
                default :
                    throw new Exception("floortype is invalid or has not been implemented in this method");
            }
        }
    }
    private FloorType _floorType;
    private int _roomAmount;

    //

    public Floor(FloorType floorType)
    {
        Entry = null;
        Exit = null;
        _roomAmount = 0;
        _floorType = floorType;
    }


    // only ever meant to be called during creation of the floor
    public void AppendRoom(Room room)
    {
        Room _currentRoom = Entry;
        if (_currentRoom == null)
        {
            Entry = room;
            Exit = room;
        }
        else
        {
            while (_currentRoom.NextRoom is not null)
            {
                _currentRoom = _currentRoom.NextRoom;
            }
            room.PreviousRoom = _currentRoom;
            _currentRoom.NextRoom = room;
            Exit = room;
        }
        _roomAmount += 1;
    }
}
