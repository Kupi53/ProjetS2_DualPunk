using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.SocialPlatforms;
using FishNet.Object;
using UnityEditor.Rendering;


public class EffectTilesController : MonoBehaviour
{
    public List<EffectTile> EffectTiles;
    private Grid _grid;

    void Start()
    {
        _grid = this.gameObject.GetComponent<Grid>();
    }
    void Update()
    {        
        Vector3Int player1Position;
        Vector3Int player2Position;
        // check if player 1 is on an effect tile, if not do some stuff also
        if (GameManager.Instance.Player1 is not null && GameManager.Instance.LocalPlayer == GameManager.Instance.Player1 )
        {
            bool player1StandingOnEFfectTile = false;
            player1Position = _grid.WorldToCell(GameManager.Instance.Player1.transform.position);
            PlayerState player1State = GameManager.Instance.Player1.GetComponent<PlayerState>();
            foreach(EffectTile effectTile in EffectTiles)
            {
                if (player1Position == effectTile.Position)
                {
                    player1StandingOnEFfectTile = true;
                    // FloorExitTile
                    if (effectTile is FloorExitTile)
                    {
                        if (Input.GetButtonDown("Pickup"))
                        {
                            effectTile.Action(GameManager.Instance.Player1);
                            if (GameManager.Instance.Player2 is not null)
                            {
                                EffectTileNetworkWrapper.Instance.EffectTileActionFromOtherPlayerRpc(GameManager.Instance.Player2.GetComponent<NetworkObject>().Owner, player1Position);
                            }
                        }
                    }
                    // RoomExitTile
                    else if (effectTile is RoomExitTile)
                    {
                        if (FloorNetworkWrapper.Instance.LocalFloorManager.CurrentRoom.IsCleared)
                            player1State.CanBeTeleported = false;
                            if (GameManager.Instance.Player2 is not null)
                            {
                                    EffectTileNetworkWrapper.Instance.EffectTileActionFromOtherPlayerRpc(GameManager.Instance.Player2.GetComponent<NetworkObject>().Owner, player1Position);
                                    GameManager.Instance.Player2.GetComponent<PlayerState>().CanBeTeleported = false;
                            }
                    }
                    else
                    {
                        effectTile.Action(GameManager.Instance.Player1);
                    }
                }
            }
            if (!player1StandingOnEFfectTile)
            {
                player1State.CanBeTeleported = true;
            }
        }
        // check if player 2 is on an effect tile, if not do some stuff also
        if (GameManager.Instance.Player2 is not null && GameManager.Instance.LocalPlayer == GameManager.Instance.Player2)
        {
            bool player2StandingOnEFfectTile = false;
            player2Position = _grid.WorldToCell(GameManager.Instance.Player2.transform.position); 
            PlayerState player2State = GameManager.Instance.Player2.GetComponent<PlayerState>(); 
            foreach(EffectTile effectTile in EffectTiles)
            {
                player2StandingOnEFfectTile = true;
                // FloorExitTile
                if (effectTile is FloorExitTile)
                {
                    if (Input.GetKeyDown("Pickup"))
                    {
                        effectTile.Action(GameManager.Instance.Player2);
                        EffectTileNetworkWrapper.Instance.EffectTileActionFromOtherPlayerRpc(GameManager.Instance.Player1.GetComponent<NetworkObject>().Owner, player2Position);
                    }
                    else if (effectTile is RoomExitTile)
                    {
                        if (FloorNetworkWrapper.Instance.LocalFloorManager.CurrentRoom.IsCleared)
                            player2State.CanBeTeleported = false;
                            if (GameManager.Instance.Player1 is not null)
                            {
                                    EffectTileNetworkWrapper.Instance.EffectTileActionFromOtherPlayerRpc(GameManager.Instance.Player1.GetComponent<NetworkObject>().Owner, player2Position);
                                    GameManager.Instance.Player1.GetComponent<PlayerState>().CanBeTeleported = false;
                            }
                    }
                    else
                    {
                        effectTile.Action(GameManager.Instance.Player2);
                    }
                }
            }
            if (!player2StandingOnEFfectTile)
            {
                player2State.CanBeTeleported = true;
            }
        }
    }
    
    #nullable enable
    public EffectTile? GetTileStoodOn(Vector3Int pos)
    {
        foreach (EffectTile effectTile in EffectTiles)
        {
            if (pos == effectTile.Position)
            {
                return effectTile;
            }
        }
        return null;
    }
    #nullable disable
}