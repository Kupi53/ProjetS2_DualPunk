using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LootTable
{
    private int _lootRange;
    public int MinLootRollAmount;
    public int MaxLootRollAmount;
    public string[] IdTable;
    public int[] MinIntervalTable;
    public int[] MaxIntervalTable;
    private Dictionary<(int,int), string> _table;

    public void Init()
    {
        _table = new Dictionary<(int, int), string>();
        for (int i = 0; i<IdTable.Length; i++)
        {
            _table[(MinIntervalTable[i], MaxIntervalTable[i])] = IdTable[i];
        }
        _lootRange = FindMaxRange(_table);
    }

    public string PickLoot()
    {
        int lootRoll = UnityEngine.Random.Range(0, _lootRange);
        (int, int) key = (0,0);
        foreach ((int,int) range in _table.Keys)
        {
            if (range.Item1 <= lootRoll && lootRoll <= range.Item2)
            {
                key = range;
            }
        }
        if (key == (0,0)) throw new System.Exception("Did not find range in the loot table");
        else return _table[key];
    }


    private static int FindMaxRange(Dictionary<(int,int), string> table)
    {
        int max = 0;
        foreach ((int, int) range in table.Keys)
        {
            if (range.Item2 > max){
                max = range.Item2;
            }
        }
        return max+1; // random range is exclusive
    }


}