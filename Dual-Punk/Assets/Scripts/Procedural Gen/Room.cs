using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WallCardinal{
    North,
    East,
    South,
    West,
    None
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
    private WallCardinal _entryWall;
    private WallCardinal _exitWall;
    private Floor _floor;
    //private
    public int _roomPrefabId;
    private bool _isCleared;
    // private
    public RoomType _roomType
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
    public void Init(int roomPrefabId, Floor floor)
    {
        _roomPrefabId = roomPrefabId;
        _floor = floor;
        _isCleared = false;
    }

    // deactives current room , activates the next by calling SwitchRoom
    public void Exit(){

    }

    
}
