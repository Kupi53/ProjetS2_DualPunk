using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;


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
        if (GameManager.Instance.Player1 is not null)
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
                    if (effectTile is RoomExitTile)
                    {
                        player1State.CanBeTeleported = false;
                        if (GameManager.Instance.Player2 is not null)
                        {
                            effectTile.Action(GameManager.Instance.Player2);
                            GameManager.Instance.Player2.GetComponent<PlayerState>().CanBeTeleported = false;
                        }
                    }
                }
            }
            if (!player1StandingOnEFfectTile)
            {
                player1State.CanBeTeleported = true;
            }
        }
        // check if player 2 is on an effect tile, if not do some stuff also
        if (GameManager.Instance.Player2 is not null)
        {
            bool player2StandingOnEFfectTile = false;
            player2Position = _grid.WorldToCell(GameManager.Instance.Player2.transform.position); 
            PlayerState player2State = GameManager.Instance.Player1.GetComponent<PlayerState>(); 
            foreach(EffectTile effectTile in EffectTiles)
            {
                if (player2Position == effectTile.Position)
                {
                    player2StandingOnEFfectTile = true;
                    effectTile.Action(GameManager.Instance.Player2);
                    if (effectTile is RoomExitTile)
                    {
                        player2State.CanBeTeleported = false;
                        effectTile.Action(GameManager.Instance.Player1);
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
}