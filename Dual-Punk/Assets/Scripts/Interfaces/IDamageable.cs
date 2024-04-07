using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Heal(int amount, float time);

    void Damage(int amount, float time);

    void SetHealth(int amount);
}
