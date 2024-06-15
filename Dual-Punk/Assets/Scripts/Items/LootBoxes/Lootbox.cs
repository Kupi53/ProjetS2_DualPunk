using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class Lootbox : MonoBehaviour
{
    private LootTableController _lootTableController;
    private bool onLootbox;
    void Start()
    {
        _lootTableController = GetComponent<LootTableController>();
    }

    void Update()
    {
        if (onLootbox && Input.GetButton("Pickup"))
        {
            Loot();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!(other.gameObject.GetComponent<NetworkObject>().Owner == GameManager.Instance.LocalPlayer.GetComponent<NetworkObject>().Owner)) return;
        onLootbox = true;

    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (!(other.gameObject.GetComponent<NetworkObject>().Owner == GameManager.Instance.LocalPlayer.GetComponent<NetworkObject>().Owner)) return;
        onLootbox = false;
    }

    void Loot()
    {
        _lootTableController.Loot();
        Destroy(gameObject);
    }
}
