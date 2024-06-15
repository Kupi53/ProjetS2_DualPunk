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
        int lootRolls = UnityEngine.Random.Range(lootTable.MinLootRollAmount, lootTable.MaxLootRollAmount+1);
        for (int i = 0; i<lootRolls; i++)
        {
            string idToSpawn = lootTable.PickLoot();
            Vector3 offset = PickOffset();
            ObjectSpawner.Instance.SpawnObjectFromIdRpc(idToSpawn, gameObject.transform.position + offset, quaternion.identity);
        }
    }

    public static Vector3 PickOffset()
    {
        int i = UnityEngine.Random.Range(-1, 2);
        int j = UnityEngine.Random.Range(-1, 2);
        return new Vector3(i, j, 0);
    }
}