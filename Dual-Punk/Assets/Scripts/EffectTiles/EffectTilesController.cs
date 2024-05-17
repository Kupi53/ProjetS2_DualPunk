using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.SocialPlatforms;
using FishNet.Object;


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
                    effectTile.Action(GameManager.Instance.Player1);
                    // Special cases
                    // RoomExitTile
                    if (effectTile is RoomExitTile)
                    {
                        player1State.CanBeTeleported = false;
                        if (GameManager.Instance.Player2 is not null)
                        {
                            EffectTileNetworkWrapper.Instance.EffectTileActionFromOtherPlayerRpc(GameManager.Instance.Player2.GetComponent<NetworkObject>().Owner);
                            GameManager.Instance.Player2.GetComponent<PlayerState>().CanBeTeleported = false;
                        }
                    }
                    // FloorExitTile
                    
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
                if (player2Position == effectTile.Position)
                {
                    player2StandingOnEFfectTile = true;
                    effectTile.Action(GameManager.Instance.Player2);
                    if (effectTile is RoomExitTile)
                    {
                        player2State.CanBeTeleported = false;
                        EffectTileNetworkWrapper.Instance.EffectTileActionFromOtherPlayerRpc(GameManager.Instance.Player1.GetComponent<NetworkObject>().Owner);
                        GameManager.Instance.Player1.GetComponent<PlayerState>().CanBeTeleported = false;

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
    public EffectTile? GetTileStoodOn(GameObject player)
    {
        foreach (EffectTile effectTile in EffectTiles)
        {
            if (_grid.WorldToCell(player.transform.position) == effectTile.Position){
                return effectTile;
            }
        }
        return null;
    }
    #nullable disable
}