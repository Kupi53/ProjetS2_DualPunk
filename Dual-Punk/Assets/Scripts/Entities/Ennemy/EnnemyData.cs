using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Ennemy", order = 1)]
public class EnnemyData : ScriptableObject
{
    public int Health;
    public float Damage;
    public float Speed;
}
