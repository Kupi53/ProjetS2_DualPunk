using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OrderInLayerController : MonoBehaviour
{
    private Grid _grid;
    private Tilemap _tilemap;
    private int _orderInLayer { get => this.gameObject.GetComponent<TilemapRenderer>().sortingOrder; } 

    void Start()
    {
        _grid = this.gameObject.GetComponentInParent<Grid>();
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
    }
    void ChangeOrderInLayer(GameObject player)
    {
        player.GetComponent<SpriteRenderer>().sortingOrder = _orderInLayer+1;
    }
}
