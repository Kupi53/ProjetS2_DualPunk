using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Ennemy", order = 1)]
public class EnnemyData : ScriptableObject
{
    public int Health;
    public float Damage;
    public float Speed;
    public int MaxLootRollAmount;
    public string[] IdTable = new string[10];
    public int[] MinIntervalTable = new int[10];
    public int[] MaxIntervalTable = new int[10];
}