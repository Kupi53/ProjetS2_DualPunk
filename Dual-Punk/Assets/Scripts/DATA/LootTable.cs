using UnityEngine;
using System.Collections.Generic;

public class LootTable
{
    private int _lootRange;
    private Dictionary<(int,int), string> _table;

    public LootTable(Dictionary<(int, int), string> table)
    {
        _table = table;
        _lootRange = FindMaxRange(table);
    }

    public string PickLoot()
    {
        int lootRoll = UnityEngine.Random.Range(0, _lootRange);
        (int, int) key = (0,0);
        foreach ((int,int) range in _table.Keys)
        {
            if (range.Item1 <= lootRoll && lootRoll <= range.Item2)
            {
                Debug.Log(lootRoll);
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