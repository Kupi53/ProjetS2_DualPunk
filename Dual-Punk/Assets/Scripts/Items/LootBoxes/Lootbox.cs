using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class Lootbox : MonoBehaviour
{
    private LootTableController _lootTableController;
    void Start()
    {
        _lootTableController = GetComponent<LootTableController>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!(other.gameObject.GetComponent<NetworkObject>().Owner == GameManager.Instance.LocalPlayer.GetComponent<NetworkObject>().Owner)) return;
        if (Input.GetButtonDown("Pickup"))
        {
            Loot();
        }
    }

    void Loot()
    {
        _lootTableController.Loot();
        Destroy(gameObject);
    }
}
