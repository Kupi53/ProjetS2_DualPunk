using Unity.Mathematics;
using UnityEngine;

public class LootTableController : MonoBehaviour
{
    [SerializeField] public LootTable lootTable;

    void Start()
    {
        lootTable.Init();
    }

    public void Loot()
    {
        int lootRolls = UnityEngine.Random.Range(lootTable.MinLootRollAmount, lootTable.MaxLootRollAmount);
        for (int i =0; i<lootRolls; i++)
        {
            string idToSpawn = lootTable.PickLoot();
            ObjectSpawner.Instance.SpawnObjectFromIdRpc(idToSpawn, gameObject.transform.position, new quaternion());
        }
    }
}