using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OrderInLayerController : MonoBehaviour
{
    private Grid _grid;
    private Tilemap _tilemap;
    private Room _room;
    private int _orderInLayer { get => this.gameObject.GetComponent<TilemapRenderer>().sortingOrder; } 

    void Start()
    {
        _grid = this.gameObject.GetComponentInParent<Grid>();
        _room = this.gameObject.GetComponentInParent<Room>();
        _tilemap = this.gameObject.GetComponent<Tilemap>();
    }
    void Update()
    {
        CheckTile();
    }


    void CheckTile()
    {
        if (GameManager.Instance.Player1 is not null)
        {
            Vector3Int player1Pos = _grid.WorldToCell(GameManager.Instance.Player1.transform.position);
            if (_tilemap.HasTile(player1Pos))
            {
                ChangeOrderInLayer(GameManager.Instance.Player1);
            }
        }
        if (GameManager.Instance.Player2 is not null)
        {
            Vector3Int player2Pos = _grid.WorldToCell(GameManager.Instance.Player2.transform.position);
            if (_tilemap.HasTile(player2Pos))
            {
                ChangeOrderInLayer(GameManager.Instance.Player2);
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
    }
    void ChangeOrderInLayer(GameObject entity)
    {
        entity.GetComponent<SpriteRenderer>().sortingOrder = _orderInLayer+1;
    }
}
