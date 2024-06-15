using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OrderInLayerController : MonoBehaviour
{
    private Grid _grid;
    private Room _room;
    private Tilemap _tilemap;

    private int _orderInLayer { get => this.gameObject.GetComponent<TilemapRenderer>().sortingOrder; } 


    private void Start()
    {
        _grid = this.gameObject.GetComponentInParent<Grid>();
        _room = this.gameObject.GetComponentInParent<Room>();
        _tilemap = this.gameObject.GetComponent<Tilemap>();
    }

    private void Update()
    {
        CheckTile();
    }


    private void CheckTile()
    {
        if (GameManager.Instance.Player1 is not null)
        {
            Vector3Int player1Pos = _grid.WorldToCell(GameManager.Instance.Player1.transform.position);
            if (_tilemap.HasTile(player1Pos))
            {
                ChangeOrderInLayer(GameManager.Instance.Player1);
                if (GameManager.Instance.Player1State.HoldingWeapon)
                {
                    ChangeOrderInLayer(GameManager.Instance.Player1State.WeaponScript.gameObject);
                }
            }
        }
        if (GameManager.Instance.Player2 is not null)
        {
            Vector3Int player2Pos = _grid.WorldToCell(GameManager.Instance.Player2.transform.position);
            if (_tilemap.HasTile(player2Pos))
            {
                ChangeOrderInLayer(GameManager.Instance.Player2);
                if (GameManager.Instance.Player2State.HoldingWeapon)
                {
                    ChangeOrderInLayer(GameManager.Instance.Player2State.WeaponScript.gameObject);
                }
            }
        }
        foreach(GameObject enemy in _room.Enemies)
        {
            if (enemy is not null)
            {
                Vector3Int enemyPos = _grid.WorldToCell(enemy.transform.position);
                if (_tilemap.HasTile(enemyPos))
                {
                    ChangeOrderInLayer(enemy);
                }
            }
        }
        foreach(GameObject item in GameObject.FindGameObjectsWithTag("Item"))
        {
            Vector3Int itemPos = _grid.WorldToCell(item.transform.position);
            if (_tilemap.HasTile(itemPos))
            {
                ChangeOrderInLayer(item);
            }
        }
    }


    private void ChangeOrderInLayer(GameObject entity)
    {
        if (entity.GetComponent<SpriteRenderer>() != null)
            entity.GetComponent<SpriteRenderer>().sortingOrder = _orderInLayer+1;
    }
}
