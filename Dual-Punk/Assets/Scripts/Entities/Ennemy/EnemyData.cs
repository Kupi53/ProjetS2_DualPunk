using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObject/Ennemy", order = 1)]
public class EnemyData : ScriptableObject
{
    public int Health;
    public float Damage;
    public float Speed;
}
